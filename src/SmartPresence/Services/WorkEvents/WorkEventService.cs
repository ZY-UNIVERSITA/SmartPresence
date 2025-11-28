using Microsoft.EntityFrameworkCore;
using SmartPresence.Services.WorkEvents.Command;
using SmartPresence.Services.WorkEvents.Model;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPresence.Services.WorkEvents
{
    public class WorkEventService : IWorkEventService
    {
        private readonly SmartPresenceDbContext _context;

        public WorkEventService(SmartPresenceDbContext context)
        {
            _context = context;
        }

        private async Task<int> GetWorkEventStatusByName(WorkEventStatusName name)
        {
            return await _context.WorkEventTypeStatus
                .Where(x => x.Name.Equals(name))
                .Select(y => y.Id)
                .FirstOrDefaultAsync();
        }

        private async Task<int> GetWorkEventTypeByName(WorkEventTypeEnum name)
        {
            return await _context.WorkEventTypes
                .Where(x => x.Name.Equals(name))
                .Select(y => y.Id)
                .FirstOrDefaultAsync();
        }

        public async Task CreateNewWorkEvent(CreateNewWorkRequestCommand workEvent)
        {
            var workEventStatusPending = workEvent.WorkEventType is WorkEventTypeEnum.REMOTE ? WorkEventStatusName.APPROVED : WorkEventStatusName.PENDING;
            var idWorkEventStatusPending = await this.GetWorkEventStatusByName(workEventStatusPending);

            var idWorkEventType = await this.GetWorkEventTypeByName(workEvent.WorkEventType);

            var newWorkEvent = new WorkEvent()
            {
                IdEmployee = workEvent.Id,
                StartDate = workEvent.BeginDate,
                EndDate = workEvent.EndDate,
                IdWorkEventStatus = idWorkEventStatusPending,
                IdWorkEventType = idWorkEventType
            };

            _context.WorkEvents.Add(newWorkEvent);
            await _context.SaveChangesAsync();
        }
    }
}

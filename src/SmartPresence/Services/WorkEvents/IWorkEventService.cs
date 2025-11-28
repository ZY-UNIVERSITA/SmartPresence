using SmartPresence.Services.WorkEvents.Command;
using System.Threading.Tasks;

namespace SmartPresence.Services.WorkEvents
{
    public interface IWorkEventService
    {
        public Task CreateNewWorkEvent(CreateNewWorkRequestCommand workEvent);
    }
}

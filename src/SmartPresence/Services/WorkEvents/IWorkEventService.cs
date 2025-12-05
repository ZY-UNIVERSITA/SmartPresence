using SmartPresence.Services.Employees.Queries;
using SmartPresence.Services.WorkEvents.Command;
using SmartPresence.Services.WorkEvents.Queries;
using System.Threading.Tasks;

namespace SmartPresence.Services.WorkEvents
{
    public interface IWorkEventService
    {
        public Task HandleNewWorkEvent(CreateNewWorkRequestCommand workEvent);
        public Task<bool> ValidateNewWorkEvent(ValidateNewWorkEventResponse workEvent);
        public Task<EmployeePendingWorkEventsResponse> GetEmployeeWorkEventsPending(EmployeePendingWorkEventsRequest request);
        public Task HandleAllWorkEventsDecisions(HandleAllWorkEventsDecisionsCommand command);
    }
}

using SmartPresence.Services.Employees.Queries;
using SmartPresence.Services.WorkEvents.Command;
using SmartPresence.Services.WorkEvents.Queries;
using System.Threading.Tasks;

namespace SmartPresence.Services.WorkEvents
{
    public interface IWorkEventService
    {
        public Task HandleNewWorkEvent(CreateNewWorkRequestCommand workEvent);
        public Task<EmployeePendingWorkEventsResponse> GetEmployeeWorkEventsPending(EmployeePendingWorkEventsRequest request);
        public Task HandleAllWorkEventsDecisions(HandleAllWorkEventsDecisionsCommand command);
        public Task HandleRemoteDays(UpdateRemoteDaysCommand command);
        public Task HandleSingleWorkEventDecision(HandleSingleWorkEventDecisionCommand command);
        public Task<RemoteDaysResponse> GetEmployeeRemoteDays(RemoteDaysRequest request);
        public Task<WorkEventsCountResponse> GetEmployeeWorkEventPendingTotal(WorkEventsCountRequest request);

    }
}

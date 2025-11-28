using SmartPresence.Services.WorkEvents.Model;
using System;

namespace SmartPresence.Services.WorkEvents.Queries
{
    public class WorkEventResponse
    {
        public int Id { get; set; }
        public int IdEmployee { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public WorkEventStatus WorkEventStatus { get; set; }
        public WorkEventType WorkEventType { get; set; }

        public WorkEventResponse(WorkEvent workEvent)
        {
            Id = workEvent.Id;
            IdEmployee = workEvent.IdEmployee;
            StartDate = workEvent.StartDate;
            EndDate = workEvent.EndDate;
            WorkEventStatus = workEvent.WorkEventStatus;
            WorkEventType = workEvent.WorkEventType;
        }
    }
}

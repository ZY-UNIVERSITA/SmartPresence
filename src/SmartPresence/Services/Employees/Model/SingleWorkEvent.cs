using SmartPresence.Services.WorkEvents.Model;
using System;

namespace SmartPresence.Services.Employees.Model
{
    public class SingleWorkEvent
    {
        public int IdEvent { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Notes { get; set; }
        //public WorkEventStatusName Status { get; set; }
        public WorkEventTypeName Type { get; set; }

        public SingleWorkEvent(Employee employee, WorkEvent workEvent)
        {
            IdEvent = workEvent.Id;
            Surname = employee.Surname;
            Name = employee.Name;
            BeginDate = workEvent.StartDate;
            EndDate = workEvent.EndDate;
            Notes = workEvent.Notes;
            //Status = workEvent.WorkEventStatus.Name;
            Type = workEvent.WorkEventType.Name;
        }
    }
}

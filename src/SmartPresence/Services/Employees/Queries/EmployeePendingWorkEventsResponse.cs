using SmartPresence.Services.Employees.Model;
using System.Collections.Generic;

namespace SmartPresence.Services.Employees.Queries
{
    public class EmployeePendingWorkEventsResponse
    {
        public List<SingleWorkEvent> ListEvents { get; set; } = new List<SingleWorkEvent>();
    }
}

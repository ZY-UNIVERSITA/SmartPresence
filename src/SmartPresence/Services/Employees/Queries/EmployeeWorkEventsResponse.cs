using SmartPresence.Services.WorkEvents.Queries;
using System.Collections.Generic;
using System.Linq;

namespace SmartPresence.Services.Employees.Queries
{
    public class EmployeeWorkEventsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Team { get; set; }
        public List<WorkEventResponse> WorkEventsList { get; set; }

        public EmployeeWorkEventsResponse(Employee employee)
        {
            Id = employee.Id;
            Name = employee.Name;
            Surname = employee.Surname;
            Team = employee.Team.Name;
            WorkEventsList = employee.WorkEvents.Select(x => new WorkEventResponse(x)).OrderBy(y => y.StartDate).ToList();
        }
    }
}

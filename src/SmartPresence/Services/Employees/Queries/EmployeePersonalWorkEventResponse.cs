using SmartPresence.Services.Employees.Model;
using SmartPresence.Services.Shared;
using SmartPresence.Services.WorkEvents.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartPresence.Services.Employees.Queries
{
    public class EmployeePersonalWorkEventResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string FullName => $"{Name} {Surname}";

        public ContractType ContractType { get; set; }
        public Role Role { get; set; }
        public Team Team { get; set; }
        public Area Area { get; set; }
        public EmployeeTimeOff TimeOff { get; set; }
        public List<WorkEventResponse> WorkEvents { get; set; }

        public EmployeePersonalWorkEventResponse(Employee employee) {
            Id = employee.Id;
            Name = employee.Name;
            Surname = employee.Surname;
            ContractType = employee.ContractType;
            Role = employee.Role;
            Team = employee.Team;
            Area = employee.Team.Area;
            TimeOff = employee.EmployeeTimeOffs.FirstOrDefault();
            WorkEvents = employee.WorkEvents.Select(x => new WorkEventResponse(x)).ToList();
        }
    }
}

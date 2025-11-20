using SmartPresence.Services.Shared;
using SmartPresence.Services.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartPresence.Services.Employees.Queries
{
    public class GetEmployeesInfoResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime HireDate { get; set; }
        public ContractType ContractType { get; set; }
        public Role Role { get; set; }
        public Team Team { get; set; }

        public GetEmployeesInfoResponse(Employee employee)
        {
            Id = employee.Id;
            Name = employee.Name;
            Surname = employee.Surname;
            BirthDate = employee.BirthDate;
            HireDate = employee.HireDate;
            ContractType = employee.ContractType;
            Role = employee.Role;
            Team = employee.Team;
        }
    }
}

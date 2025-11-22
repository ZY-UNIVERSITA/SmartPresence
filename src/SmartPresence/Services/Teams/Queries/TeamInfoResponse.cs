using SmartPresence.Services.Employees;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartPresence.Services.Teams.Queries
{
    public class TeamInfoResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IdArea { get; set; }
        public IEnumerable<Employee> Employees { get; set; }

        public TeamInfoResponse(Team team)
        {
            Id = team.Id;
            Name = team.Name;
            IdArea = team.IdArea;
            Employees = team.Employees;
        }
    }
}

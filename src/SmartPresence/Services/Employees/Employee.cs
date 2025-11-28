using SmartPresence.Services.Employees.Model;
using SmartPresence.Services.Shared;
using SmartPresence.Services.WorkEvents.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPresence.Services.Employees
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime HireDate { get; set; }

        public int IdContractType { get; set; }

        [ForeignKey(nameof(IdContractType))]
        [InverseProperty("Employees")]
        public ContractType ContractType { get; set; }

        public int IdRole { get; set; }

        [ForeignKey(nameof(IdRole))]
        [InverseProperty("Employees")]
        public Role Role { get; set; }

        public int IdTeam { get; set; }

        [ForeignKey(nameof(IdTeam))]
        [InverseProperty("Employees")]
        public Team Team { get; set; }

        public IEnumerable<WorkEvent> WorkEvents { get; set; }
    }
}

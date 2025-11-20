using SmartPresence.Services.Employees;
using SmartPresence.Services.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SmartPresence.Services.Teams
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int IdArea { get; set; }

        [ForeignKey(nameof(IdArea))]
        [InverseProperty("Teams")]
        public Area Area { get; set; }
    
        public IEnumerable<Employee> Employees { get; set; }   
    }
}

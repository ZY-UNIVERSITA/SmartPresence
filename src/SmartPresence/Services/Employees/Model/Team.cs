using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPresence.Services.Employees.Model
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

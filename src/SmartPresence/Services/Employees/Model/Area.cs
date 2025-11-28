using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartPresence.Services.Employees.Model
{
    public class Area
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<Team> Teams { get; set; }
    }
}

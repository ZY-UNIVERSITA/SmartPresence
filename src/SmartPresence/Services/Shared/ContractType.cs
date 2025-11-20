using SmartPresence.Services.Employees;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartPresence.Services.Shared
{
    public class ContractType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int DailyHours { get; set; }
        public int WeeklyDays { get; set; }

        public IEnumerable<Employee> Employees { get; set; }
    }
}

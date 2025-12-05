using SmartPresence.Services.Employees;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

    public static class LeaveHolidaysHours
    {
        public static int Days { get; private set; } = 26;
        public static int Hours { get; private set; } = 120;
        public static int StandardWorkingHours { get; private set; } = 40;
    }
}

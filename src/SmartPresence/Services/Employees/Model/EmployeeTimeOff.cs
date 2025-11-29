using System;
using System.Collections.Generic;
using System.Text;

namespace SmartPresence.Services.Employees.Model
{
    public class EmployeeTimeOff
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int HolidayAccrued { get; set; }
        public int HolidayUsed { get; set; }
        public int HolidayTotal { get; set; }
        public int LeaveAccrued { get; set; }
        public int LeaveUsed { get; set; }
        public int LeaveTotal { get; set; }
    }
}

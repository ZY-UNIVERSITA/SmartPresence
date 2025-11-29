using SmartPresence.Services.Employees.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartPresence.Services.Employees.Queries
{
    public class EmployeeTimeOffByIdAndYearResponse
    {
        public int Year { get; set; }
        public int HolidayAccrued { get; set; }
        public int HolidayUsed { get; set; }
        public int HolidayTotal { get; set; }
        public int LeaveAccrued { get; set; }
        public int LeaveUsed { get; set; }
        public int LeaveTotal { get; set; }

        public EmployeeTimeOffByIdAndYearResponse(EmployeeTimeOff employeeTimeOff)
        {
            Year = employeeTimeOff.Year;
            HolidayTotal = employeeTimeOff.HolidayTotal;
            HolidayAccrued = employeeTimeOff.HolidayAccrued;
            HolidayUsed = employeeTimeOff.HolidayUsed;
            LeaveTotal = employeeTimeOff.LeaveTotal;
            LeaveAccrued = employeeTimeOff.LeaveAccrued;
            LeaveUsed = employeeTimeOff.LeaveUsed;
        }
    }
}

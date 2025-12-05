using SmartPresence.Services.Employees.Model;

namespace SmartPresence.Services.Employees.Queries
{
    public class EmployeeTimeOffByIdAndYearResponse
    {
        public int Year { get; set; }
        public int HolidayAccrued { get; set; }
        public int HolidayUsed { get; set; }
        public int HolidayTotal { get; set; }
        public double LeaveAccrued { get; set; }
        public double LeaveUsed { get; set; }
        public double LeaveTotal { get; set; }

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

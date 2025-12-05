using SmartPresence.Services.Shared;

namespace SmartPresence.Services.Employees.Model
{
    public class EmployeeTimeOff
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int HolidayAccrued { get; set; }
        public int HolidayUsed { get; set; }
        public int HolidayTotal { get; set; }
        public double LeaveAccrued { get; set; }
        public double LeaveUsed { get; set; }
        public double LeaveTotal { get; set; }

        public EmployeeTimeOff()
        {
        }

        public EmployeeTimeOff(ContractType contract, int idEmployee, int year)
        {
            Id = idEmployee;
            Year = year;
            HolidayUsed = 0;
            HolidayAccrued = 0;
            HolidayTotal = LeaveHolidaysHours.Days * (contract.DailyHours * contract.WeeklyDays) / LeaveHolidaysHours.StandardWorkingHours;
            LeaveUsed = 0;
            LeaveAccrued = 0;
            LeaveTotal = LeaveHolidaysHours.Hours * (contract.DailyHours * contract.WeeklyDays) / LeaveHolidaysHours.StandardWorkingHours;
        }
    }
}

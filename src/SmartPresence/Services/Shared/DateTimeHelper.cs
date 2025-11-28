using System;

namespace SmartPresence.Services.Shared
{
    public static class DateTimeHelper
    {
        public static DateTime GetMonday(DateTime date)
        {
            // calcolo la distanza dal lunedi
            // c# usa 0 per sunday fino a 6 per saturday
            var mondayOff = DayOfWeek.Monday - date.DayOfWeek;

            // perciò se oggi cade di domenica allora si avrebbe 1-0 = 1 quindi per ottenere domenica ovvero 6 (0 = lunedy e 6 = domenica)
            // bisogna sottrarre -7.
            // 1 - 7 = -6 che andrà aggiunto alla data passata per ottenere il lunedi
            if (mondayOff.Equals(1))
            {
                mondayOff -= 7;
            }

            var mondayDate = date.AddDays(mondayOff);

            return mondayDate;
        }

        public static DateTime GetFirstDayOfTheMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static int GetNumbersOfDaysInWeek()
        {
            return Enum.GetValues<DayOfWeek>().Length;
        }

        public static int GetDaysBetweenTwoDates(DateTime beginDate, DateTime endDate, CalendarViewName? calendarView = null)
        {
            return calendarView switch
            {
                CalendarViewName.WEEK => DateTimeHelper.GetNumbersOfDaysInWeek(),
                CalendarViewName.MONTH => DateTime.DaysInMonth(beginDate.Year, beginDate.Month),
                CalendarViewName.CUSTOM => (endDate.Date - beginDate.Date).Days + 1,
                _ => (endDate.Date - beginDate.Date).Days + 1
            };
        }

        public static DateTime GetFirstDateByCalendarView(CalendarViewName calendarView, DateTime date)
        {
            return calendarView switch
            {
                CalendarViewName.WEEK => DateTimeHelper.GetMonday(date),
                CalendarViewName.MONTH => DateTimeHelper.GetFirstDayOfTheMonth(date),
                CalendarViewName.CUSTOM => date,
                _ => date
            };
        }

        public static DateTime GetLastDate(DateTime beginDate, int days)
        {
            return beginDate.AddDays(days);
        }
    }

}

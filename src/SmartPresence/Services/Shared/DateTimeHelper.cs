using System;

namespace SmartPresence.Services.Shared
{
    public static class DateTimeHelper
    {
        // Ritorna il lunedi dato un certo giorno
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

        // Ritorna il primo giorno del mese
        public static DateTime GetFirstDayOfTheMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        // Ritorna il numero di giorni in un certo mese conoscendo la data
        public static int GetDaysInAMonth(DateTime date)
        {
            return DateTime.DaysInMonth(date.Year, date.Month);
        }

        // Ritorna il numero di giorni in una settimana
        public static int GetNumbersOfDaysInWeek()
        {
            return Enum.GetValues<DayOfWeek>().Length;
        }

        // Restituisce il numero di giorni effettivi (comprensivi di data di inizio e data di fine) tra 2 date
        public static int GetDaysBetweenTwoDates(DateTime beginDate, DateTime endDate, CalendarViewName? calendarView = null)
        {
            return calendarView switch
            {
                CalendarViewName.WEEK => DateTimeHelper.GetNumbersOfDaysInWeek(),
                CalendarViewName.MONTH => DateTimeHelper.GetDaysInAMonth(beginDate),
                CalendarViewName.CUSTOM => (endDate.Date - beginDate.Date).Days + 1,
                _ => (endDate.Date - beginDate.Date).Days + 1
            };
        }

        // Restitusce il primo giorno dato il calendario:
        // Lunedi se la vista è week
        // Il primo giorno del mese se è month
        // Altrimenti restituisce il giorno stesso passato
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

        // Restituisce l'ultimo giorno data la vista
        // La domenica se è week
        // L'ultimo giorno del mese se è month
        // Altrimenti restituisce il giorno stesso
        public static DateTime GetLastDayByCalendarView(CalendarViewName calendarView, DateTime date)
        {
            var firstDay = DateTimeHelper.GetFirstDateByCalendarView(calendarView, date);
            return calendarView switch
            {
                CalendarViewName.WEEK => firstDay.AddDays(DateTimeHelper.GetNumbersOfDaysInWeek() - 1),
                CalendarViewName.MONTH => firstDay.AddDays(DateTimeHelper.GetDaysInAMonth(firstDay) - 1),
                CalendarViewName.CUSTOM => date,
                _ => date,
            };
        }
    }

}

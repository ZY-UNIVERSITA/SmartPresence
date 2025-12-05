using System;
using System.Collections.Generic;
using System.Globalization;

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

        // Funzione che serve per calcolare i giorni in cui sono presenti sabato, domenica e festività
        public static List<CalendarDay> GetDaysWithHolidays(DateTime beginDate, DateTime endDate)
        {
            // Ottieni le festività dell'anno corrente ed eventualmente dell'anno successivo se l'enddate è in un altro anno
            var holidaysBeginDate = Holidays.GetAllHolidaysByYear(beginDate.Year);
            var holidaysEndDate = endDate.Year.Equals(beginDate.Year) ? holidaysBeginDate : Holidays.GetAllHolidaysByYear(endDate.Year);

            // Ora si calcolano i giorni tra begin e end
            var days = DateTimeHelper.GetDaysBetweenTwoDates(beginDate, endDate);

            var list = new List<CalendarDay>();

            // Per ognuno di questi giorni, si crea un oggetto contenente:
            // giorno della settimana
            // numero del mese
            // data copmleta in formato yyyy-MM-dd
            // se il giorno è lavorativo oppure è un festività
            for (int i = 0; i < days; i++)
            {
                var currentDay = beginDate.AddDays(i);
                var calendarDay = new CalendarDay()
                {
                    DayOfWeek = days <= DateTimeHelper.GetNumbersOfDaysInWeek()
                        ? currentDay.ToString("ddd", CultureInfo.InvariantCulture).ToUpper()
                        : currentDay.ToString("ddd", CultureInfo.InvariantCulture).Substring(0, 1).ToUpper(),

                    // Numero del mese 
                    NumberOfDay = currentDay.Day.ToString(),

                    // Data completa in formato anno-mese-giorno
                    Date = currentDay.Date.ToString("yyyy-MM-dd"),
                    Holiday = holidaysBeginDate.Contains(currentDay) || holidaysEndDate.Contains(currentDay)
                        || currentDay.DayOfWeek.Equals(DayOfWeek.Sunday) || currentDay.DayOfWeek.Equals(DayOfWeek.Saturday)
                        ? DayType.HOLIDAY : DayType.WORK
                };
                list.Add(calendarDay);
            }

            return list;
        }
    }

    public static class Holidays
    {
        public static List<DateTime> GetAllHolidaysByYear(int year)
        {
            var easter = Holidays.GetEasterDate(year);    // Pasqua

            return new List<DateTime>
            {
                new DateTime(year, 1, 1),   // 1 gennaio
                new DateTime(year, 1, 6),   // 6 gennaio
                new DateTime(year, 4, 25),  // 25 aprile
                new DateTime(year, 5, 1),   // 1 maggio
                new DateTime(year, 6, 2),   // 2 giugno
                new DateTime(year, 8, 15),  // 15 agosto
                new DateTime(year, 11, 1),  // 1 novembre
                new DateTime(year, 12, 8),  // 8 dicembre
                new DateTime(year, 12, 25), // 25 dicembre
                new DateTime(year, 12, 26),  // 26 dicembre
                easter,                     // pasqua
                easter.AddDays(1)           // pasquetta
            };
        }

        // Algoritmo di Gauss per calcolare pasqua
        public static DateTime GetEasterDate(int year)
        {
            int a = year % 19;
            int b = year / 100;
            int c = year % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * l) / 451;
            int month = (h + l - 7 * m + 114) / 31;
            int day = ((h + l - 7 * m + 114) % 31) + 1;

            return new DateTime(year, month, day);
        }
    }

    public enum DayType
    {
        WORK,
        HOLIDAY
    }
}

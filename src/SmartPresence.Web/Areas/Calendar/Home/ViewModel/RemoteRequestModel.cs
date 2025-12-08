using Microsoft.AspNetCore.Http;
using SmartPresence.Services.Shared;
using SmartPresence.Services.WorkEvents.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace SmartPresence.Web.Areas.Calendar.Home.ViewModel
{
    public class RemoteRequestModel 
    {
        [Required]
        public int IdEmployee { get; set; }
        public string RepeatRemote { get; set; }
        public string Days { get; set; } = string.Empty;
        public string DaysNextWeek { get; set; } = string.Empty;

        [Required]
        public CalendarViewName CalendarViewFilter { get; set; }
        [Required]
        public DateTime BeginDateSearch { get; set; }
        [Required]
        public DateTime EndDateSearch { get; set; }

        public UpdateRemoteDaysCommand ToCommand()
        {
            return new UpdateRemoteDaysCommand()
            {
                IdEmployee = IdEmployee,
                Days = this.CreateDaysList(Days),
                DaysNextWeek = this.CreateDaysList(DaysNextWeek),
                Repeat = !string.IsNullOrWhiteSpace(RepeatRemote),
            };
        }

        private List<DateTime> CreateDaysList(string days)
        {
            return string.IsNullOrWhiteSpace(days) ? new List<DateTime>() : days.Split(",")
                .Select<string, DateTime?>(x =>
                    DateTime.TryParseExact(x, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) ? result : null)
                .Where(y => y.HasValue)
                .Select(z => z.Value)
                .Order()
                .ToList();
        }

        public SearchModel ToSearchModel()
        {
            return new SearchModel()
            {
                CalendarView = CalendarViewFilter,
                BeginDateString = BeginDateSearch.Date.ToString("yyyy-MM-dd"),
                EndDateString = EndDateSearch.Date.ToString("yyyy-MM-dd"),
            };
        }
    }
}
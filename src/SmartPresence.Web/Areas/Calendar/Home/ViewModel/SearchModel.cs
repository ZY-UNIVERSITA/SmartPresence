using SmartPresence.Services.Employees.Queries;
using SmartPresence.Services.Shared;
using SmartPresence.Services.WorkEvents.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable enable
namespace SmartPresence.Web.Areas.Calendar.Home.ViewModel
{
    public class SearchModel : IValidatableObject
    {
        [EnumDataType(typeof(CalendarViewName))]
        public CalendarViewName CalendarView { get; set; }
        public string? BeginDateString { get; set; }
        public string? EndDateString { get; set; }
        private DateTime? BeginDate { get; set; }
        private DateTime? EndDate { get; set; }

        public SearchModel()
        {
            CalendarView = CalendarViewName.WEEK;
        }

        // Aggiunge una validazione al modello passata alla get
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Se presente, il begin date string deve essere trasformabile in una data
            if (!string.IsNullOrWhiteSpace(BeginDateString))
            {
                if (DateTime.TryParse(BeginDateString, out DateTime begin))
                {
                    BeginDate = begin;
                }
                else
                {
                    yield return new ValidationResult(
                        "Begin date is not in a valid format.",
                        new[] { nameof(BeginDateString) }
                    );
                }
            }

            // Se presente, l'end date string deve essere trasformabile in una data
            if (!string.IsNullOrWhiteSpace(EndDateString))
            {
                if (DateTime.TryParse(EndDateString, out DateTime end))
                {
                    EndDate = end;
                }
                else
                {
                    yield return new ValidationResult(
                        "End date is not in a valid format.",
                        new[] { nameof(EndDateString) }
                    );
                }
            }

            // Se presente, l'end date non deve essere precedente al begin date
            if (BeginDate.HasValue && EndDate.HasValue && EndDate < BeginDate)
            {
                yield return new ValidationResult(
                    "End date cannot be earlier than the start date.",
                    new[] { nameof(EndDate), nameof(BeginDate) }
                );
            }
        }

        // Crea la query di ricerca
        public EmployeeWorkEventsRequest ToQuery(int idEmployee)
        {
            // Crea la base usando id dell'employee e fai un filter-out per gli eventi rifiutati
            var employeeWorkEventRequest = new EmployeeWorkEventsRequest()
            {
                IdEmployee = idEmployee,
                WorkEventStatusFilterOut = WorkEventStatusName.REFUSED
            };

            // Controllare che le date di inizio e fine visualizzazione siano valide
            if (BeginDate is null || EndDate is null)
            {
                // Se la vista scelta è quella custom, restituisci di default quella settimanale
                // Se è già settimanale oppure mensile non fare nulla
                CalendarView = CalendarView.Equals(CalendarViewName.CUSTOM) ? CalendarViewName.WEEK : CalendarView;

                // Crea delle nuove date di inizio e fine valide
                BeginDate = DateTimeHelper.GetFirstDateByCalendarView(CalendarView, DateTime.UtcNow);
                EndDate = DateTimeHelper.GetLastDayByCalendarView(CalendarView, DateTime.UtcNow);
            }

            employeeWorkEventRequest.BeginDate = BeginDate.Value;
            employeeWorkEventRequest.EndDate = EndDate.Value;

            return employeeWorkEventRequest;
        }

    }
}

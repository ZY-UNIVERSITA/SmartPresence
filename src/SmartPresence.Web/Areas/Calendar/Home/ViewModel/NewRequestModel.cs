using SmartPresence.Services.Shared;
using SmartPresence.Services.WorkEvents;
using SmartPresence.Services.WorkEvents.Command;
using SmartPresence.Services.WorkEvents.Model;
using SmartPresence.Services.WorkEvents.Queries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPresence.Web.Areas.Calendar.Home.ViewModel
{
    public class NewRequestModel : IValidatableObject
    {
        [Required]
        public int IdEmployee { get; set; }
        [Required]
        public CalendarViewName CalendarViewFilter { get; set; }
        [Required]
        public DateTime BeginDateSearch { get; set; }
        [Required]
        public DateTime EndDateSearch { get; set; }
        [Required]
        public DateTime BeginDateRequest { get; set; }
        [Required]
        public DateTime EndDateRequest { get; set; }
        [Required]
        public WorkEventTypeName EventType { get; set; }
        public string NotesRequest { get; set; }

        // Esegue la validazione del modello
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // La data di fine della richiesta non può mai essere inferiore alla data di inizio
            if (EndDateRequest < BeginDateRequest)
            {
                yield return new ValidationResult(
                    "End date cannot be earlier than the begin date.",
                    new[] { nameof(EndDateRequest), nameof(BeginDateRequest) }
                );
            }

            var holidaysBetweenDates = DateTimeHelper.FindHolidaysBetweenDates(BeginDateRequest, EndDateRequest);
            if (holidaysBetweenDates.Any())
            {
                yield return new ValidationResult(
                    $"Selected dates contains holidays. You cannot include them, for example: {holidaysBetweenDates[0].ToString("yyyy-MM-dd")}.",
                    new[] { nameof(EndDateRequest), nameof(BeginDateRequest) }
                );
            }
        }

        // Esegue una seconda validazione prima di provare ad eseguire qualsiasi inserimento
        public async Task<List<string>> AsyncValidation(IValidationWorkEventService validationWorkEventService)
        {
            var validationResult = new List<string>();

            /// DA PORTARE NEL METODO DI VALIDAZIONE
            // Aggiunge gli orari di inizio e fine delle ferie che corrispondono all'inizio dell'orario e alla fine dell'orario lavorativo
            if (EventType.Equals(WorkEventTypeName.HOLIDAY))
            {
                BeginDateRequest = BeginDateRequest.AddHours(9);
                EndDateRequest = EndDateRequest.AddHours(18);
            }

            // Se le date di inizio e fine sono valide, esegue una validazione per il database
            if (BeginDateRequest <= EndDateRequest)
            {
                var validationRequest = new ValidateNewWorkEventResponse()
                {
                    BeginDate = BeginDateRequest,
                    EndDate = EndDateRequest,
                    IdEmployee = IdEmployee,
                    WorkEventTypeName = EventType
                };

                // Cerca se sono presenti uno o più eventi già presenti che si sovrappongono a quello da inserire
                var validationProblems = await validationWorkEventService.ValidateNewWorkEvent(validationRequest);

                // Se ci sono altri eventi, allora aggiungi il messaggio di errore
                Action<string> addToValidationResult = x => validationResult.Add(x);
                validationProblems.ForEach(addToValidationResult);
            }

            return validationResult;
        }

        // Crea il modello di ricerca
        public SearchModel ToSearchmodel()
        {
            var searchModel = new SearchModel()
            {
                CalendarView = CalendarViewName.CUSTOM,
                BeginDateString = BeginDateRequest.Date.AddDays(-1).ToString("yyyy-MM-dd"),
                EndDateString = EndDateRequest.Date.AddDays(1).ToString("yyyy-MM-dd"),
            };

            return searchModel;
        }

        // Crea il commando usato per inserire l'evento
        public CreateNewWorkRequestCommand ToCommand()
        {
            var command = new CreateNewWorkRequestCommand()
            {
                IdEmployee = IdEmployee,
                WorkEventType = EventType,
                BeginDate = BeginDateRequest,
                EndDate = EndDateRequest,
                Notes = NotesRequest
            };

            return command;
        }
    }

}

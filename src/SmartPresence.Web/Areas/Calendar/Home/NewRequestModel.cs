using SmartPresence.Services.Shared;
using SmartPresence.Services.WorkEvents;
using SmartPresence.Services.WorkEvents.Command;
using SmartPresence.Services.WorkEvents.Model;
using SmartPresence.Services.WorkEvents.Queries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SmartPresence.Web.Areas.Calendar.Home
{
    public class NewRequestModel : IValidatableObject
    {
        [Required]
        public int IdEmployee { get; set; }

        [Required]
        public CalendarViewName CalendarViewFilter { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime BeginDateRequest { get; set; }
        public DateTime EndDateRequest { get; set; }
        public WorkEventTypeName EventType { get; set; }

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
        }

        // Esegue una seconda validazione prima di provare ad eseguire qualsiasi inserimento
        public async Task<List<string>> AsyncValidation(IWorkEventService workEventService)
        {
            var validationResult = new List<string>();

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
                var areThereAnyOtherEvents = await workEventService.ValidateNewWorkEvent(validationRequest);

                // Se ci sono altri eventi, allora aggiungi il messaggio di errore
                if (areThereAnyOtherEvents)
                {
                    validationResult.Add("Cannot add an event where there is another which will be overlapping.");
                }
            }

            return validationResult;
        }

        // Crea il modello di ricerca
        public SearchModel ToSearchmodel()
        {
            var searchModel = new SearchModel()
            {
                CalendarView = CalendarViewFilter,
                BeginDateString = BeginDate.Date.ToString("yyyy-MM-dd"),
                EndDateString = EndDate.Date.ToString("yyyy-MM-dd"),
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
            };

            return command;
        }
    }

}

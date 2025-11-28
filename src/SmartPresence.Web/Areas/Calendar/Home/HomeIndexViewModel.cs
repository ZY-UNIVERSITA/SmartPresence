using SmartPresence.Services.Employees.Queries;
using SmartPresence.Services.Shared;
using SmartPresence.Services.WorkEvents.Model;
using SmartPresence.Services.WorkEvents.Queries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartPresence.Web.Areas.Calendar.Home
{
    public class HomeIndexViewModel
    {
        public int Id { get; set; }
        public List<EmployeeIdNameAndEvents> Employees { get; set; }
        public List<string> TeamList { get; set; }
        public List<string> EventTypeList { get; set; }
        public CalendarViewName CalendarViewFilter { get; set; }
        public List<CalendarDay> Calendar { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        private int DaysBetweenBeginAndEndDate { get; set; }
        public DateTime Today { get; set; } = DateTime.Now;

        // Prepara l'header della tabella
        public void PrepareCalendarHeader()
        {
            // A seconda della view, calcola il numero di giorni da visualizzare
            DaysBetweenBeginAndEndDate = DateTimeHelper.GetDaysBetweenTwoDates(BeginDate, EndDate, CalendarViewFilter);

            // Crea una lista che contenga i giorni con la rispettiva settimana da visualizzare
            Calendar = Enumerable.Range(0, DaysBetweenBeginAndEndDate)
                .Select(x => new CalendarDay()
                {
                    // Giorno della settimana: nome completo del giorno se il numero di giorni da visualizare è 7 o inferiore
                    // altrimenti visualizza solo la prima lettera
                    DayOfWeek = DaysBetweenBeginAndEndDate <= DateTimeHelper.GetNumbersOfDaysInWeek()
                        ? BeginDate.AddDays(x).ToString("ddd").ToUpper()
                        : BeginDate.AddDays(x).ToString("ddd").Substring(0, 1).ToUpper(),

                    // Numero del mese 
                    NumberOfDay = BeginDate.AddDays(x).Day.ToString(),

                    // Data completa in formato anno-mese-giorno
                    Date = BeginDate.AddDays(x).Date.ToString("yyyy-MM-dd")
                }).ToList();
        }

        // Prepara la lista di dati da visualizzare
        public void PrepareViewData(List<EmployeeWorkEventsResponse> response)
        {
            // Prepara la lista dei dati dei dipendenti
            Employees = response
                .Select(x => new EmployeeIdNameAndEvents()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Surname = x.Surname,
                    Team = x.Team,
                    Events = CreateEmployeeSingleDayEvents(x.WorkEventsList, x)
                }).ToList();

            // Prepara la lista dei dati sui team che verranno usati come filtro
            TeamList = response
                .Select(x => x.Team)
                .Distinct()
                .ToList();

            // Prepara la lista dei dati sui tipi di eventi che verranno usati come filtro
            EventTypeList = response
                .SelectMany(x => x.WorkEventsList
                .Select(y => y.WorkEventType.Name.ToString()))
                .Distinct()
                .ToList();
        }

        // Crea la lista degli eventi per ogni singolo dipendente
        private List<SingleDayEvents> CreateEmployeeSingleDayEvents(List<WorkEventResponse> response, EmployeeWorkEventsResponse x)
        {
            // Crea una lista degli eventi
            var everyDaysEvents = new List<SingleDayEvents>();

            // Questa variabile rappresenta il numero di giorni effettivamente utilizzati rispetto ai totali
            var dayUsed = 0;

            // Fai un ciclo sugli eventi restituiti dalla query 
            for (int i = 0; i < response.Count; i++)
            {
                var singleEvent = response[i];

                // Calcola la data minima e la data massima che rappresenta la data di inizio e di fine di visualizzazione sul calendario dell'evento.
                // E' fondamentale se la data dell'evento inizia prima e/o finisce dopo il range di date che verranno visualizzate nel calendario
                var minDate = singleEvent.StartDate < BeginDate ? BeginDate : singleEvent.StartDate;
                var maxDate = singleEvent.EndDate > EndDate ? EndDate : singleEvent.EndDate;

                // Calcola il numero effettivi di giorni in cui esiste l'evento
                var dayBetweenMinAndMaxDate = DateTimeHelper.GetDaysBetweenTwoDates(minDate.Date, maxDate.Date);

                // Calcola il numero di giorni tra l'inizio dell'evento e l'ultimo giorno in cui è successo almeno 1 evento
                // Questo serve per aggiungere dei giorni vuoti tra 2 eventi (solo nel caso in cui ci fossero giorni di mezzo senza eventi)
                var daysBetweenDayUsedAndMinDate = DateTimeHelper.GetDaysBetweenTwoDates(BeginDate.Date, minDate.Date) - 1;

                // Riempi i giorni vuoti in mezzo a 2 eventi in cui non ci sono eventi
                while (dayUsed < daysBetweenDayUsedAndMinDate)
                {
                    everyDaysEvents.Add(new SingleDayEvents()
                    {
                        Date = BeginDate.AddDays(dayUsed).Date,
                        Days = 1,
                    });

                    dayUsed++;
                }


                // Crea una variabile che contiene la lista di eventi per un certo giorno
                SingleDayEvents newSingleEvent = null;

                // Crea una variabile per definire se ci sono più eventi per lo stesso giorno
                bool sameDayEvent = false;

                // Gli unici eventi che possono accadere lo stesso giorno sono remote e il permesso
                if (singleEvent.WorkEventType.Name.Equals(WorkEventTypeName.LEAVE)
                    || singleEvent.WorkEventType.Name.Equals(WorkEventTypeName.REMOTE))
                {
                    // Cerca se esiste già un evento tra questi tipi già inseriti nello stesso giorno del nuovo evento da inserire
                    var searchSingleDayEvents = everyDaysEvents.Where(x => x.Date.Date.Equals(minDate.Date)).FirstOrDefault();

                    // Se ne esiste già 1 allora aggiorna il riferimento al giorno con quello trovato e aggiorna la variabile booleana a true 
                    if (searchSingleDayEvents is not null)
                    {
                        newSingleEvent = searchSingleDayEvents;
                        sameDayEvent = !sameDayEvent;
                    }
                }

                // Se non ci sono già eventi leave/remote per lo stesso giorno, crea il giorno stesso
                if (newSingleEvent is null)
                {
                    newSingleEvent = new SingleDayEvents()
                    {
                        Date = minDate,
                        Days = dayBetweenMinAndMaxDate,
                        ListEvents = new List<EventTypeAndStatus>()
                    };
                }

                // Aggiunge il nuovo evento alla lista degli eventi per il determinato giorno
                newSingleEvent.ListEvents.Add(new EventTypeAndStatus()
                {
                    Type = singleEvent.WorkEventType.Name.ToString(),
                    Status = singleEvent.WorkEventStatus.Name.ToString()
                });

                // Se il giorno è nuovo perchè non ha mai avuti eventi, allora bisogna pusharlo nella lista dei giorni
                // E bisogna aggiornare il contatore dei giorni usati
                if (!sameDayEvent)
                {
                    everyDaysEvents.Add(newSingleEvent);
                    dayUsed += dayBetweenMinAndMaxDate;
                }

            }

            // Se mancano dei giorni tra l'ultimo evento presente e la fine del periodo prescelto, aggiungici degli eventi vuoti
            while (dayUsed < DaysBetweenBeginAndEndDate)
            {
                everyDaysEvents.Add(new SingleDayEvents()
                {
                    Date = BeginDate.AddDays(dayUsed).Date,
                    Days = 1
                });

                dayUsed++;
            }

            return everyDaysEvents;
        }
    }

    public class CalendarDay
    {
        public string DayOfWeek { get; set; }
        public string NumberOfDay { get; set; }
        public string Date { get; set; }
    }

    public class EmployeeIdNameAndEvents
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Team { get; set; }
        public List<SingleDayEvents> Events { get; set; }
    }

    public class SingleDayEvents
    {
        public DateTime Date { get; set; }
        public int Days { get; set; }
        public List<EventTypeAndStatus> ListEvents { get; set; }
    }

    public class EventTypeAndStatus
    {
        public string Type { get; set; }
        public string Status { get; set; }
    }
}

using SmartPresence.Services.Employees.Model;
using SmartPresence.Services.Shared;
using SmartPresence.Services.WorkEvents.Model;
using System;

namespace SmartPresence.Services.Employees.Queries
{
    // Richiesta dei work event del dipendente
    // Bisogna passargli:
    // Id employee
    // WorkEventStatusFilterOut: rappresenta il tipo di richiesta da filtrare (è sempre null se la richiesta non arriva dalla home)
    // BeginDate: data inizio
    // EndDate: data fine

    public class EmployeeWorkEventsRequest
    {
        public int IdEmployee { get; set; }
        public WorkEventStatusName? WorkEventStatusFilterOut { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

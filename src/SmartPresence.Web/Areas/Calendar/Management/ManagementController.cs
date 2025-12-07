using Microsoft.AspNetCore.Mvc;
using SmartPresence.Services.Employees.Queries;
using SmartPresence.Services.Users;
using SmartPresence.Services.WorkEvents;
using SmartPresence.Services.WorkEvents.Command;
using SmartPresence.Services.WorkEvents.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPresence.Web.Areas.Calendar.Management
{
    [Area("Calendar")]
    public partial class ManagementController : AuthenticatedBaseController
    {
        private readonly IWorkEventService _workEventService;
        private readonly IUserService _userService;

        public ManagementController(IWorkEventService workEventService, IUserService userService)
        {
            _workEventService = workEventService;
            _userService = userService;
        }

        [HttpGet]
        public async virtual Task<IActionResult> Index()
        {
            var email = Identita.EmailUtenteCorrente;
            var userId = _userService.GetId(new Services.Users.Queries.UserIdentificationRequest(email));

            var request = new EmployeePendingWorkEventsRequest()
            {
                IdEmployee = userId,
            };

            var query = await _workEventService.GetEmployeeWorkEventsPending(request);

            return View(query);
        }

        [HttpPost]
        public virtual async Task<IActionResult> HandleSingleRequest(int id, WorkEventStatusName status)
        {
            try
            {
                await _workEventService.HandleSingleWorkEventDecision(
                    new HandleSingleWorkEventDecisionCommand
                    {
                        WorkEventId = id,
                        Status = status
                    }
                );
                return Ok(new { success = true, message = $"Request {status.ToString().ToLower()}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async virtual Task<IActionResult> AcceptAllRequests(string listId, WorkEventStatusName status)
        {
            // Il client passa una stringa contenente gli id in formato (id1-id2-id3)
            // La funzione separa la stringa a partire da "-" in singole stringhe
            // e prova fare il parsing della stringa in un che restituisce 2 cose: bool isNumber e il valore int
            // Seleziona solo lista dei valori validi e da questa lista seleziona solo il numero
            var listIdNumber = listId
                .Split("-")
                .Select(x =>
                {
                    bool isNumber = int.TryParse(x, out var number);
                    return (isNumber, number);
                }).
                Where(y => y.isNumber)
                .Select(z => z.number)
                .ToList();

            await _workEventService.HandleAllWorkEventsDecisions(new Services.WorkEvents.Command.HandleAllWorkEventsDecisionsCommand()
            {
                ListId = listIdNumber,
                Status = status
            });

            return RedirectToAction("Index");
        }
    }
}

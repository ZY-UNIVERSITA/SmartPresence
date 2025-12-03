using Microsoft.AspNetCore.Mvc;
using SmartPresence.Services.Employees.Queries;
using SmartPresence.Services.Users;
using SmartPresence.Services.WorkEvents;
using SmartPresence.Services.WorkEvents.Model;
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

        public async virtual Task<IActionResult> Post(int id, WorkEventStatusName status)
        {
            await _workEventService.HandleWorkEventDecision(new Services.WorkEvents.Command.HandleWorkEventDecisionCommand()
            {
                Id = id,
                Status = status
            });

            return RedirectToAction("Index");
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using SmartPresence.Services.Employees;
using SmartPresence.Services.Users;
using SmartPresence.Services.WorkEvents;
using SmartPresence.Web.Infrastructure;
using System.Threading.Tasks;

namespace SmartPresence.Web.Areas.Calendar.Home
{
    [Area("Calendar")]
    public partial class HomeController : AuthenticatedBaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IUserService _userService;
        private readonly IWorkEventService _workEventService;

        public HomeController(IEmployeeService employeeService, IUserService userService, IWorkEventService workEventService)
        {
            _employeeService = employeeService;
            _userService = userService;
            _workEventService = workEventService;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index(SearchModel searchModel)
        {
            // Trova l'utente a partire dall'email di login
            var email = Identita.EmailUtenteCorrente;
            var userId = _userService.GetId(new Services.Users.Queries.UserIdentificationRequest(email));

            // Se model è nullo allora crea uno di default
            searchModel ??= new SearchModel();

            // Valida il search model
            if (!ModelState.IsValid)
            {
                Alerts.AddError(this, "End date cannot be happening before the begin date.");
                // Se è invalido, usa quello di default
                searchModel = new SearchModel();
            }

            // Fai la query per ottenere i dati per popolare il calendario
            var employeeWorkEventRequest = searchModel.ToQuery(userId);
            var queryResult = await _employeeService.GetAllEmployeeWorkEvents(employeeWorkEventRequest);

            // Passa i dati alla Index View Model che prepara i dati ottenuti dalla query per la view
            var viewModel = new HomeIndexViewModel()
            {
                Id = userId,
                BeginDate = employeeWorkEventRequest.BeginDate,
                EndDate = employeeWorkEventRequest.EndDate,
                CalendarViewFilter = searchModel.CalendarView,
            };
            viewModel.PrepareCalendarHeader();
            viewModel.PrepareViewData(queryResult);

            // Restituisci la pagina all'utente
            return View(viewModel);
        }
    }
}

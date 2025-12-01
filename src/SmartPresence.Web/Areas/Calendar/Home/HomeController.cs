using Microsoft.AspNetCore.Mvc;
using SmartPresence.Services.Employees;
using SmartPresence.Services.Employees.Queries;
using SmartPresence.Services.Users;
using SmartPresence.Services.WorkEvents;
using SmartPresence.Web.Infrastructure;
using System;
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

            // Fai una seconda query per ottenere le statistiche dei permessi e delle ferie
            var employeeTimeOffRequest = new EmployeeTimeOffByIdAndYearRequest()
            {
                Id = userId,
                Year = DateTime.Now.Year
            };
            var queryResultTimeOff = await _employeeService.GetEmployeeTimeOff(employeeTimeOffRequest);

            // Passa i dati alla Index View Model che prepara i dati ottenuti dalla query per la view
            var viewModel = new HomeIndexViewModel()
            {
                IdEmployee = userId,
                BeginDate = employeeWorkEventRequest.BeginDate,
                EndDate = employeeWorkEventRequest.EndDate,
                CalendarViewFilter = searchModel.CalendarView
            };
            viewModel.PrepareCalendarHeader();
            viewModel.PrepareViewData(queryResult, queryResultTimeOff);

            // Restituisci la pagina all'utente
            return View(viewModel);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CreateNewRequest(NewRequestModel model)
        {
            // Crea un search model che rifletta la view dell'utente in quel momento
            var searchModel = model.ToSearchmodel();

            // Crea il comando per inserire una nuova data
            var command = model.ToCommand();

            // Aggiungi il nuovo work event
            await _workEventService.CreateNewWorkEvent(command);

            // Restituisci la stessa pagina in cui l'utente si trova
            return RedirectToAction("Index", searchModel);
        }

    }
}

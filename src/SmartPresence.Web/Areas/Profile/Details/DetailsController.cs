using Microsoft.AspNetCore.Mvc;
using SmartPresence.Services.Employees;
using SmartPresence.Services.Employees.Model;
using SmartPresence.Services.Users;
using System;
using System.Threading.Tasks;

namespace SmartPresence.Web.Areas.Profile.Details
{
    [Area("Profile")]
    public partial class DetailsController : AuthenticatedBaseController
    {
        public readonly IEmployeeService _employeeService;
        public readonly IUserService _userService;

        public DetailsController(IEmployeeService employeeService, IUserService userService)
        {
            _employeeService = employeeService;
            _userService = userService;
        }

        public virtual async Task<IActionResult> Index()
        {
            var email = Identita.EmailUtenteCorrente;
            var userId = _userService.GetId(new Services.Users.Queries.UserIdentificationRequest(email));

            var queryResult = await _employeeService.GetEmployeePersonalWorkEvent(new Services.Employees.Queries.EmployeePersonalWorkEventRequest()
            {
                IdEmployee = userId,
                Year = DateTime.UtcNow.Year
            });

            return View(queryResult);
        }
    }
}

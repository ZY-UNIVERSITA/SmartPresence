using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartPresence.Services.Employees.Queries;
using SmartPresence.Services.Users;
using SmartPresence.Services.WorkEvents;
using SmartPresence.Services.WorkEvents.Queries;
using System.Threading.Tasks;

namespace SmartPresence.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class ApiController : ControllerBase
    {
        public readonly IWorkEventService _workEventService;
        public readonly IUserService _userService;

        public ApiController(IWorkEventService workEventService, IUserService userService)
        {
            _workEventService = workEventService;
            _userService = userService;
        }

        [HttpGet("events_count")]
        public virtual async Task<IActionResult> GetEventsCount(string email)
        {
            var userId = _userService.GetId(new Services.Users.Queries.UserIdentificationRequest(email));

            var count = await _workEventService.GetEmployeeWorkEventPendingTotal(new WorkEventsCountRequest() { userId = userId });

            return Ok(count);
        }
    }
}

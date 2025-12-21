using Microsoft.AspNetCore.Mvc;
using System;

namespace SmartPresence.Web.Features.Error
{
    public partial class ErrorController : Controller
    {
        [Route("Error/PageNotFound")]
        public virtual IActionResult PageNotFound()
        {
            return View("PageNotFound");
        }
    }
}

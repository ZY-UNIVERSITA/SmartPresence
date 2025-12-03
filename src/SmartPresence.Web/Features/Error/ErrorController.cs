using Microsoft.AspNetCore.Mvc;
using System;

namespace SmartPresence.Web.Features.Error
{
    public partial class ErrorController : Controller
    {
        // Action principale per errori generici
        public IActionResult Index()
        {
            return View("PageNotFound");
        }

        [Route("Error/PageNotFound")]
        public IActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View("PageNotFound");
        }

        [Route("Error/AccessDenied")]
        public IActionResult AccessDenied()
        {
            Response.StatusCode = 403;
            return View("PageNotFound");
        }
    }
}

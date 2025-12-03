using Microsoft.AspNetCore.Mvc;
using System;

namespace SmartPresence.Web.Features.Error
{
    public partial class ErrorController : Controller
    {
        // Action principale per errori generici
        public virtual IActionResult Index()
        {
            return View("PageNotFound");
        }

        [Route("Error/PageNotFound")]
        public virtual IActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View("PageNotFound");
        }

        [Route("Error/AccessDenied")]
        public virtual IActionResult AccessDenied()
        {
            Response.StatusCode = 403;
            return View("PageNotFound");
        }
    }
}

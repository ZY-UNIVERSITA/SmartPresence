using Microsoft.AspNetCore.Mvc;
using System;

namespace SmartPresence.Web.Features.Error
{
    public partial class ErrorController : Controller
    {
        public virtual IActionResult PageNotFound()
        {
            Console.WriteLine("Prova");

            return View();
        }
    }
}

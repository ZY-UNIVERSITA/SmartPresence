using Microsoft.AspNetCore.Mvc;
using SmartPresence.Services.Employees.Model;

namespace SmartPresence.Web.Areas.Profile.Details
{
    [Area("Profile")]
    public partial class DetailsController : Controller
    {
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SmartPresence.Web.Features.Home;

namespace SmartPresence.Web.Areas.Dashboard.Home
{
    [Area("Dashboard")]
    public partial class HomeController : Controller
    {
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}

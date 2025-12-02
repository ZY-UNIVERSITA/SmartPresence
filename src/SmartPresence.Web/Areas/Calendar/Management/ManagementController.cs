using Microsoft.AspNetCore.Mvc;

namespace SmartPresence.Web.Areas.Calendar.Management
{
    [Area("Calendar")]
    public partial class ManagementController : Controller
    {
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}

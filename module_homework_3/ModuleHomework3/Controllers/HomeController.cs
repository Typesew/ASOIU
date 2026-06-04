using Microsoft.AspNetCore.Mvc;

namespace ModuleHomework3.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace Backend3.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Backend3.Models;
using Backend3.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend3.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsersService _usersService;
        public AccountController(IUsersService usersService)
        {
            _usersService = usersService;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost] // POST запрос принимает заполненную данными с формы модель
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _usersService.Register(model);
                    return RedirectToAction("Index", "Event");
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("RegistrationErrors", ex.Message);
                }
            }
            return View(model); 
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _usersService.Login(model);
                    return RedirectToAction("Index", "Event");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Errors", ex.Message);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _usersService.Logout();
            return RedirectToAction("Index", "Event");
        }

    }
}

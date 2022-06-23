using Backend3.Models;
using Backend3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend3.Controllers
{
    public class UserController : Controller
    {
        private readonly IAccountService _accountService;
        public UserController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var ev = await _accountService.GetUser(id, User.Identity.Name);
                return View(ev);
            }
            catch
            {
                return RedirectToAction("Index", "Event");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _accountService.GetUserView(id);
            return View(user);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _accountService.Edit(model);
            return RedirectToAction("Details", new {model.Id});
        }
    }
}

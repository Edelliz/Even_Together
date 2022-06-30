using Backend3.Models;
using Backend3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend3.Controllers
{
    public class GroupController : Controller
    {
        private readonly IGroupService _groupService;
        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }



        [HttpGet]
        [Authorize] //(Role ="Пользователь")
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateGroupViewModel model, Guid eventId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string userEmail = User.Identity.Name;
            await _groupService.Create(model, userEmail, eventId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendInvitation(Guid eventId, Guid userId)
        {
            string userEmail = User.Identity.Name;
            await _groupService.SendInvitation(userEmail, userId, eventId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendRequest(Guid groupId, Guid userId)
        {
            string userEmail = User.Identity.Name;
            await _groupService.SendRequest(userEmail, groupId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AcceptRequest(Guid groupId, Guid userId)
        {
            string userEmail = User.Identity.Name;
            await _groupService.AcceptRequest(userEmail, userId, groupId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AcceptInvitation(Guid groupId)
        {
            string userEmail = User.Identity.Name;
            await _groupService.AcceptInvitation(groupId, userEmail);
            return RedirectToAction("Index");
        }
    }
}

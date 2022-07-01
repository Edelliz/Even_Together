using Backend3.Models;
using Backend3.Services;
using Backend3.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend3.Controllers
{
    public class GroupController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public GroupController(IGroupService groupService, UserManager<User> userManager, ApplicationDbContext context)
        {
            _groupService = groupService;
            _userManager = userManager;
            _context = context;
        }



        [HttpGet]
        [Authorize] //(Role ="Пользователь")
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateGroupViewModel model, Guid id)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.GetUserAsync(User);

            await _groupService.Create(model, user, id);
            return RedirectToAction("Details", "Event", new { id });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendInvitation(Guid id, Guid userId)
        {
            string userEmail = User.Identity.Name;
            await _groupService.SendInvitation(userEmail, userId, id);
            return RedirectToAction("Details","Event", new { id });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendRequest(Guid groupId, Guid userId, Guid id)
        {
            string userEmail = User.Identity.Name;
            await _groupService.SendRequest(userEmail, groupId);
            return RedirectToAction("Details", "Event", new { id } );
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RefuseRequest(Guid groupId, Guid userId)
        {
            string userEmail = User.Identity.Name;
            await _groupService.RefuseRequest(userEmail, userId, groupId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RefuseInvitation(Guid groupId)
        {
            string userEmail = User.Identity.Name;
            await _groupService.RefuseInvitation(groupId, userEmail);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CancelInvitation(Guid groupId, Guid userId, Guid id)
        {
            var invitation = _context.Invitations.FirstOrDefault(x => x.GroupId == groupId && x.UserId == userId);
            _context.Invitations.Remove(invitation);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Event", new {id });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CancelRequest(Guid groupId, Guid userId, Guid id)
        {
            var request = _context.Request.FirstOrDefault(x => x.GroupId == groupId && x.UserId == userId);
            _context.Request.Remove(request);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Event", new { id });
        }
    }
}

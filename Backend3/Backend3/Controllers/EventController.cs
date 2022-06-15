using Backend3.Models;
using Backend3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend3.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }
        public async Task<IActionResult> Index(DateTime? data)
        {
            var ev = await _eventService.GetAllEvent(data);
            return View(ev);
        }


        [HttpGet]
        [Authorize] //(Role ="Организатор")
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string userEmail = User.Identity.Name;
            await _eventService.CreateEvent(model, userEmail);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                string userEmail = User.Identity.Name;
                var ev = await _eventService.GetEvent(id, userEmail);
                return View(ev);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var ev = await _eventService.GetEventView(id);
            return View(ev);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(CreateEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _eventService.ChangeEvent(model);
            return RedirectToAction("Details");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> FindCompany(Guid id)
        {
            string userEmail = User.Identity.Name;
            await _eventService.FindCompany(userEmail, id);
            return RedirectToAction("Details");
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BuyTicket(Guid id)
        {
            string userEmail = User.Identity.Name;
            await _eventService.BuyTicket(userEmail, id);
            return RedirectToAction("Details");
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Rate(Guid EventId, int grage)
        {
            await _eventService.Rate(EventId, grage);
            return RedirectToAction("Details");
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostReview(Guid EventId, string text)
        {
            string userEmail = User.Identity.Name;
            await _eventService.PostReview(EventId, text, userEmail);
            return RedirectToAction("Details");
        }
    }
}

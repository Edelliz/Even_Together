﻿using Backend3.Models;
using Backend3.Services;
using Backend3.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend3.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ApplicationDbContext _context;

        public EventController(IEventService eventService, ApplicationDbContext context)
        {
            _eventService = eventService;
            _context = context;
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
                var group = _context.Group.Include(x => x.Invitations).FirstOrDefault(x => x.Owner == userEmail && x.EventId == ev.Id);
                ViewBag.UserGroup = group;

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
            return RedirectToAction("Details", new { model.Id });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> FindCompany(Guid id)
        {
            string userEmail = User.Identity.Name;
            await _eventService.FindCompany(userEmail, id);
            return RedirectToAction("Details", new { id });
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BuyTicket(Guid id)
        {
            string userEmail = User.Identity.Name;
            await _eventService.BuyTicket(userEmail, id);
            return RedirectToAction("Details", new { id });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Rate(GradeViewModel model)
        {
            await _eventService.Rate(model.EventId, model.Grade);
            return RedirectToAction("Details", new { model.EventId });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostReview(ReviewViewModel model)
        {
            string userEmail = User.Identity.Name;
            await _eventService.PostReview(model.EventId, model.Text, userEmail);
            return RedirectToAction("Details", "Event", new { id = model.EventId });
        }
    }
}

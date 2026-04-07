using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarbandOfTheSpiritborn.Data;
using WarbandOfTheSpiritborn.Models;

namespace WarbandOfTheSpiritborn.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Everyone can view events
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? searchPhrase)
        {
            IQueryable<Events> query = _context.Events.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchPhrase))
            {
                query = query.Where(e => e.EventName != null && e.EventName.Contains(searchPhrase));
            }

            var events = await query
                .OrderBy(e => e.Date)
                .ThenBy(e => e.Time)
                .ToListAsync();

            ViewData["SearchPhrase"] = searchPhrase;

            return View(events);
        }

        // Everyone can view event details
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }

        // Only Moderator and Administrator can create events
        [Authorize(Roles = "Moderator,Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // Only Moderator and Administrator can create events
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> Create([Bind("Id,EventName,EventInfo,Time,Date")] Events eventItem)
        {
            if (!ModelState.IsValid)
            {
                return View(eventItem);
            }

            _context.Add(eventItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Only Moderator and Administrator can edit events
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }

        // Only Moderator and Administrator can edit events
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EventName,EventInfo,Time,Date")] Events eventItem)
        {
            if (id != eventItem.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(eventItem);
            }

            try
            {
                _context.Update(eventItem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(eventItem.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // Only Moderator and Administrator can delete events
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }

        // Only Moderator and Administrator can delete events
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}

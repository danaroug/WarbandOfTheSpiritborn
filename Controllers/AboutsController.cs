using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarbandOfTheSpiritborn.Data;
using WarbandOfTheSpiritborn.Models;

namespace WarbandOfTheSpiritborn.Controllers
{
    public class AboutsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AboutsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Everyone can view the About page
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.About.ToListAsync());
        }

        // Only Moderator and Administrator can create About content
        [Authorize(Roles = "Moderator,Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // Only Moderator and Administrator can create About content
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> Create([Bind("Id,AboutTitle,AboutText")] About about)
        {
            if (!ModelState.IsValid)
            {
                return View(about);
            }

            _context.Add(about);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Only Moderator and Administrator can edit About content
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var about = await _context.About.FindAsync(id);
            if (about == null)
            {
                return NotFound();
            }

            return View(about);
        }

        // Only Moderator and Administrator can edit About content
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AboutTitle,AboutText")] About about)
        {
            if (id != about.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(about);
            }

            try
            {
                _context.Update(about);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AboutExists(about.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // Only Moderator and Administrator can delete About content
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var about = await _context.About.FirstOrDefaultAsync(m => m.Id == id);
            if (about == null)
            {
                return NotFound();
            }

            return View(about);
        }

        // Only Moderator and Administrator can delete About content
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var about = await _context.About.FindAsync(id);

            if (about != null)
            {
                _context.About.Remove(about);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AboutExists(int id)
        {
            return _context.About.Any(e => e.Id == id);
        }
    }
}
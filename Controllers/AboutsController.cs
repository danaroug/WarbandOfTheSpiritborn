using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarbandOfTheSpiritborn.Data;
using WarbandOfTheSpiritborn.Models;

namespace WarbandOfTheSpiritborn.Controllers
{
    public class AboutsController : Controller
    {
        private const string ManageAboutRoles = "Moderator,Administrator";

        private readonly ApplicationDbContext _context;

        public AboutsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Everyone can view the About page.
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var aboutContent = await _context.About
                .AsNoTracking()
                .OrderBy(about => about.Id)
                .ToListAsync();

            return View(aboutContent);
        }

        // Only Moderators and Administrators can create About content.
        [Authorize(Roles = ManageAboutRoles)]
        public IActionResult Create()
        {
            return View();
        }

        // Only Moderators and Administrators can create About content.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ManageAboutRoles)]
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

        // Only Moderators and Administrators can edit About content.
        [Authorize(Roles = ManageAboutRoles)]
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

        // Only Moderators and Administrators can edit About content.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ManageAboutRoles)]
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

        // Only Moderators and Administrators can delete About content.
        [Authorize(Roles = ManageAboutRoles)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var about = await _context.About
                .AsNoTracking()
                .FirstOrDefaultAsync(about => about.Id == id);

            if (about == null)
            {
                return NotFound();
            }

            return View(about);
        }

        // Only Moderators and Administrators can delete About content.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ManageAboutRoles)]
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
            return _context.About.Any(about => about.Id == id);
        }
    }
}
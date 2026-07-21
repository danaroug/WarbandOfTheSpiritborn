using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarbandOfTheSpiritborn.Data;
using WarbandOfTheSpiritborn.Models;
using WarbandOfTheSpiritborn.Services;

namespace WarbandOfTheSpiritborn.Controllers
{
    public class AboutsController : Controller
    {
        private const string ManageAboutRoles = "Moderator,Administrator";

        private readonly ApplicationDbContext _context;
        private readonly IHtmlSanitizationService _htmlSanitizationService;

        public AboutsController(
            ApplicationDbContext context,
            IHtmlSanitizationService htmlSanitizationService)
        {
            _context = context;
            _htmlSanitizationService = htmlSanitizationService;
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

        // Display the form for creating About content.
        [Authorize(Roles = ManageAboutRoles)]
        public IActionResult Create()
        {
            return View();
        }

        // Save new About content.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ManageAboutRoles)]
        public async Task<IActionResult> Create(
            [Bind("Id,AboutTitle,AboutText")] About about)
        {
            // Remove unsafe HTML before storing the content.
            about.AboutText =
                _htmlSanitizationService.SanitizeAbout(about.AboutText);
            // Revalidate because sanitization may have changed the content.
            ModelState.Remove(nameof(About.AboutText));
            TryValidateModel(about);
            if (!ModelState.IsValid)
            {
                return View(about);
            }

            _context.Add(about);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Display the form for editing About content.
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

        // Save changes to existing About content.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ManageAboutRoles)]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,AboutTitle,AboutText")] About about)
        {
            if (id != about.Id)
            {
                return NotFound();
            }

            // Remove unsafe HTML before updating the database.
            about.AboutText =
                _htmlSanitizationService.SanitizeAbout(about.AboutText);

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

        // Display the delete confirmation page.
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

        // Delete the confirmed About entry.
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

        // Check whether an About entry still exists.
        private bool AboutExists(int id)
        {
            return _context.About.Any(about => about.Id == id);
        }
    }
}
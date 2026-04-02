using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarbandOfTheSpiritborn.Data;
using WarbandOfTheSpiritborn.Models;

namespace WarbandOfTheSpiritborn.Controllers
{
    public class BuildsController : Controller
    {
        private static readonly string[] Professions =
        {
            "Elementalist", "Warrior", "Ranger", "Mesmer",
            "Necromancer", "Thief", "Guardian", "Engineer"
        };

        private readonly ApplicationDbContext _context;

        public BuildsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Builds
        public async Task<IActionResult> Index()
        {
            var builds = await _context.Builds
                .AsNoTracking()
                .OrderByDescending(b => b.BuildDate)
                .ToListAsync();

            return View(builds);
        }

        // GET: Builds/ByProfession?profession=Elementalist
        public async Task<IActionResult> ByProfession(string? profession)
        {
            if (string.IsNullOrWhiteSpace(profession))
            {
                return NotFound();
            }

            var trimmedProfession = profession.Trim();
            var normalizedProfession = trimmedProfession.ToLower();

            var builds = await _context.Builds
                .AsNoTracking()
                .Where(b =>
                    b.Profession != null &&
                    b.Profession.Trim().ToLower() == normalizedProfession)
                .OrderByDescending(b => b.BuildDate)
                .ToListAsync();

            ViewData["Profession"] = trimmedProfession;

            return View(builds);
        }

        // GET: Builds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var build = await _context.Builds
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (build == null)
            {
                return NotFound();
            }

            return View(build);
        }

        // GET: Builds/Create
        [Authorize]
        public IActionResult Create()
        {
            PopulateProfessions();
            return View();
        }

        // POST: Builds/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,BuildName,Profession,ShortDescription,BuildAuthor,Item,Stat,WeaponSet,OtherItems,Rotation,MainSkills,SecondarySkills,BuildDate")] Builds build)
        {
            if (!ModelState.IsValid)
            {
                PopulateProfessions();
                return View(build);
            }

            _context.Builds.Add(build);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ByProfession), new { profession = build.Profession });
        }

        // GET: Builds/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var build = await _context.Builds.FindAsync(id);

            if (build == null)
            {
                return NotFound();
            }

            PopulateProfessions();
            return View(build);
        }

        // POST: Builds/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BuildName,Profession,ShortDescription,BuildAuthor,Item,Stat,WeaponSet,OtherItems,Rotation,MainSkills,SecondarySkills,BuildDate")] Builds build)
        {
            if (id != build.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                PopulateProfessions();
                return View(build);
            }

            try
            {
                _context.Builds.Update(build);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BuildExists(build.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(ByProfession), new { profession = build.Profession });
        }

        // GET: Builds/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var build = await _context.Builds
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (build == null)
            {
                return NotFound();
            }

            return View(build);
        }

        // POST: Builds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var build = await _context.Builds.FindAsync(id);

            if (build == null)
            {
                return NotFound();
            }

            _context.Builds.Remove(build);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool BuildExists(int id)
        {
            return _context.Builds.Any(b => b.Id == id);
        }

        private void PopulateProfessions()
        {
            ViewBag.ProfessionList = new SelectList(Professions);
        }
    }
}

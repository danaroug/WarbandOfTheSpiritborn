using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarbandOfTheSpiritborn.Data;
using WarbandOfTheSpiritborn.Models;

namespace WarbandOfTheSpiritborn.Controllers
{
    public class GalleriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GalleriesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Galleries
        public async Task<IActionResult> Index()
        {
            var galleryItems = await _context.Gallery
                .AsNoTracking()
                .ToListAsync();

            return View(galleryItems);
        }

        // GET: Galleries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var galleryItem = await _context.Gallery
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id);

            if (galleryItem == null)
            {
                return NotFound();
            }

            return View(galleryItem);
        }

        // GET: Galleries/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Galleries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> Create(GalleryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var fileName = await SaveUploadedFileAsync(model);

            if (string.IsNullOrWhiteSpace(fileName))
            {
                ModelState.AddModelError(nameof(model.Image), "Please choose an image.");
                return View(model);
            }

            var galleryItem = new Gallery
            {
                Picture = fileName
            };

            _context.Gallery.Add(galleryItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Galleries/Delete/5
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var galleryItem = await _context.Gallery
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id);

            if (galleryItem == null)
            {
                return NotFound();
            }

            return View(galleryItem);
        }

        // POST: Galleries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var galleryItem = await _context.Gallery.FindAsync(id);

            if (galleryItem == null)
            {
                return NotFound();
            }

            _context.Gallery.Remove(galleryItem);
            await _context.SaveChangesAsync();

            DeleteUploadedFile(galleryItem.Picture);

            return RedirectToAction(nameof(Index));
        }

        private async Task<string?> SaveUploadedFileAsync(GalleryViewModel model)
        {
            if (model.Image == null || model.Image.Length == 0)
            {
                return null;
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "photos");
            Directory.CreateDirectory(uploadsFolder);

            var extension = Path.GetExtension(model.Image.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await model.Image.CopyToAsync(fileStream);

            return uniqueFileName;
        }

        private void DeleteUploadedFile(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "photos");
            var filePath = Path.Combine(uploadsFolder, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}



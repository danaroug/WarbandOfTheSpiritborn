using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarbandOfTheSpiritborn.Data;
using WarbandOfTheSpiritborn.Models;
using WarbandOfTheSpiritborn.Services;

namespace WarbandOfTheSpiritborn.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHtmlSanitizationService _htmlSanitizationService;

        public BlogsController(
            ApplicationDbContext context,
            IHtmlSanitizationService htmlSanitizationService)
        {
            _context = context;
            _htmlSanitizationService = htmlSanitizationService;
        }

        // Display all blog posts, newest first.
        public async Task<IActionResult> Index()
        {
            var blogs = await _context.Blog
                .AsNoTracking()
                .OrderByDescending(b => b.ArticleDate)
                .ToListAsync();

            return View(blogs);
        }

        // Display one blog post.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blog
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // Display the form for creating a blog post.
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // Save a new blog post.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(
            [Bind("Id,BlogName,BlogPost,BlogAuthor,ArticleDate")] Blog blog)
        {
            // Remove unsafe HTML from the rich-text content.
            blog.BlogPost =
                _htmlSanitizationService.SanitizeBlog(blog.BlogPost);

            // Revalidate BlogPost because its value changed after model binding.
            ModelState.Remove(nameof(Blog.BlogPost));
            TryValidateModel(blog);

            if (!ModelState.IsValid)
            {
                return View(blog);
            }

            _context.Blog.Add(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Display the form for editing a blog post.
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blog.FindAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // Save changes to an existing blog post.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,BlogName,BlogPost,BlogAuthor,ArticleDate")] Blog blog)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            // Remove unsafe HTML before updating the database.
            blog.BlogPost =
                _htmlSanitizationService.SanitizeBlog(blog.BlogPost);

            // Revalidate BlogPost because sanitization may make it empty.
            ModelState.Remove(nameof(Blog.BlogPost));
            TryValidateModel(blog);

            if (!ModelState.IsValid)
            {
                return View(blog);
            }

            try
            {
                _context.Blog.Update(blog);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogExists(blog.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // Display the delete confirmation page.
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blog
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // Delete the confirmed blog post.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blog.FindAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            _context.Blog.Remove(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Check whether the blog post still exists.
        private bool BlogExists(int id)
        {
            return _context.Blog.Any(b => b.Id == id);
        }
    }
}
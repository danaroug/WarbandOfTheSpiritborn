using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WarbandOfTheSpiritborn.Areas.Admin.Controllers
{
    [Area("Admin")] // Must match area folder
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /Admin/Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();

            // Dictionary to store roles per user
            var userRoles = new Dictionary<string, IList<string>>();

            foreach (var user in users)
            {
                // Ensure roles list is never null
                var roles = await _userManager.GetRolesAsync(user) ?? new List<string>();
                userRoles[user.Id] = roles;
            }

            // Get all roles in system and ensure not null
            var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync() ?? new List<string>();

            ViewBag.UserRoles = userRoles;
            ViewBag.AllRoles = allRoles;
            ViewBag.Message = TempData["Message"] as string;

            return View(users);
        }

        // POST: /Admin/Admin/AssignRole
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !await _roleManager.RoleExistsAsync(role))
                return NotFound();

            await _userManager.AddToRoleAsync(user, role);
            TempData["Message"] = $"Role '{role}' assigned to {user.Email}";
            return RedirectToAction("Users");
        }

        // POST: /Admin/Admin/RemoveRole
        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !await _roleManager.RoleExistsAsync(role))
                return NotFound();

            await _userManager.RemoveFromRoleAsync(user, role);
            TempData["Message"] = $"Role '{role}' removed from {user.Email}";
            return RedirectToAction("Users");
        }

        // POST: /Admin/Admin/CreateRole
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName) || await _roleManager.RoleExistsAsync(roleName))
            {
                TempData["Message"] = "Invalid or existing role name.";
                return RedirectToAction("Users");
            }

            await _roleManager.CreateAsync(new IdentityRole(roleName));
            TempData["Message"] = $"Role '{roleName}' created successfully.";
            return RedirectToAction("Users");
        }
    }
}

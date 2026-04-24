using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WarbandOfTheSpiritborn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private const string AdministratorRole = "Administrator";

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /Admin/Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users
                .OrderBy(user => user.Email)
                .ToListAsync();

            var roles = await _roleManager.Roles
                .Where(role => role.Name != null)
                .OrderBy(role => role.Name)
                .Select(role => role.Name!)
                .ToListAsync();

            var userViewModels = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                userViewModels.Add(new AdminUserViewModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? "No email",
                    UserName = user.UserName ?? "No username",
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = userRoles.OrderBy(role => role).ToList()
                });
            }

            var viewModel = new AdminUsersPageViewModel
            {
                Users = userViewModels,
                AllRoles = roles,
                CurrentUserId = _userManager.GetUserId(User)
            };

            return View(viewModel);
        }

        // POST: /Admin/Admin/CreateRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            roleName = roleName?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(roleName))
            {
                TempData["ErrorMessage"] = "Role name cannot be empty.";
                return RedirectToAction(nameof(Users));
            }

            if (await _roleManager.RoleExistsAsync(roleName))
            {
                TempData["ErrorMessage"] = $"Role '{roleName}' already exists.";
                return RedirectToAction(nameof(Users));
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = GetIdentityErrors(result);
                return RedirectToAction(nameof(Users));
            }

            TempData["SuccessMessage"] = $"Role '{roleName}' was created successfully.";
            return RedirectToAction(nameof(Users));
        }

        // POST: /Admin/Admin/AssignRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            roleName = roleName?.Trim() ?? string.Empty;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User could not be found.";
                return RedirectToAction(nameof(Users));
            }

            if (string.IsNullOrWhiteSpace(roleName) || !await _roleManager.RoleExistsAsync(roleName))
            {
                TempData["ErrorMessage"] = "Selected role does not exist.";
                return RedirectToAction(nameof(Users));
            }

            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                TempData["ErrorMessage"] = $"{user.Email} already has the '{roleName}' role.";
                return RedirectToAction(nameof(Users));
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = GetIdentityErrors(result);
                return RedirectToAction(nameof(Users));
            }

            TempData["SuccessMessage"] = $"Role '{roleName}' was assigned to {user.Email}.";
            return RedirectToAction(nameof(Users));
        }

        // POST: /Admin/Admin/RemoveRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            roleName = roleName?.Trim() ?? string.Empty;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User could not be found.";
                return RedirectToAction(nameof(Users));
            }

            if (string.IsNullOrWhiteSpace(roleName) || !await _roleManager.RoleExistsAsync(roleName))
            {
                TempData["ErrorMessage"] = "Selected role does not exist.";
                return RedirectToAction(nameof(Users));
            }

            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                TempData["ErrorMessage"] = $"{user.Email} does not have the '{roleName}' role.";
                return RedirectToAction(nameof(Users));
            }

            if (roleName == AdministratorRole)
            {
                var administrators = await _userManager.GetUsersInRoleAsync(AdministratorRole);

                if (administrators.Count <= 1)
                {
                    TempData["ErrorMessage"] = "You cannot remove the last Administrator account.";
                    return RedirectToAction(nameof(Users));
                }

                if (user.Id == _userManager.GetUserId(User))
                {
                    TempData["ErrorMessage"] = "You cannot remove your own Administrator role.";
                    return RedirectToAction(nameof(Users));
                }
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = GetIdentityErrors(result);
                return RedirectToAction(nameof(Users));
            }

            TempData["SuccessMessage"] = $"Role '{roleName}' was removed from {user.Email}.";
            return RedirectToAction(nameof(Users));
        }

        private static string GetIdentityErrors(IdentityResult result)
        {
            return string.Join(" ", result.Errors.Select(error => error.Description));
        }
    }

    public class AdminUsersPageViewModel
    {
        public List<AdminUserViewModel> Users { get; set; } = new();

        public List<string> AllRoles { get; set; } = new();

        public string? CurrentUserId { get; set; }
    }

    public class AdminUserViewModel
    {
        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public bool EmailConfirmed { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();
    }
}
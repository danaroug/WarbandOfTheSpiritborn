using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace WarbandOfTheSpiritborn.Areas.Identity
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            await EnsureRolesAsync(roleManager); // Create missing roles.
            await EnsureAdminUserAsync(userManager); // Create or update the admin user.
        }

        private static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[]
            {
                AppRoles.Administrator,
                AppRoles.User,
                AppRoles.Moderator
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task EnsureAdminUserAsync(UserManager<IdentityUser> userManager)
        {
            var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL");
            var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                Console.WriteLine("Admin seeding skipped because credentials are missing.");
                return;
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);

                if (!createResult.Succeeded)
                {
                    foreach (var error in createResult.Errors)
                    {
                        Console.WriteLine($"Admin creation failed: {error.Description}");
                    }

                    return;
                }

                Console.WriteLine("Admin user created successfully.");
            }

            if (!await userManager.IsInRoleAsync(adminUser, AppRoles.Administrator))
            {
                await userManager.AddToRoleAsync(adminUser, AppRoles.Administrator);
                Console.WriteLine("Administrator role assigned successfully.");
            }
        }
    }
}

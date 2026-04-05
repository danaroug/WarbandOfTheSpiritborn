using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace TalesOfCoding.Areas.Identity
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "User", "Contributor", "Admin" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }

}

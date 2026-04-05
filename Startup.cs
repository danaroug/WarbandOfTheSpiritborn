using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WarbandOfTheSpiritborn.Areas.Identity;
using WarbandOfTheSpiritborn.Data;
using WarbandOfTheSpiritborn.Models;
using WarbandOfTheSpiritborn.Services;

namespace WarbandOfTheSpiritborn
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))); // Use SQL Server for the app database.

            services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true; // Require email confirmation before sign-in.
            })
                .AddRoles<IdentityRole>() // Enable role support.
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                    policy.RequireRole(AppRoles.Administrator)); // Restrict admin actions to administrators.
            });

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings")); // Bind email settings from configuration.
            services.AddTransient<IEmailSender, MailKitEmailSender>(); // Use MailKit for identity emails.
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint(); // Show EF Core migration errors in development.
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // Authenticate the current user.
            app.UseAuthorization(); // Apply role and policy checks.

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Users}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages(); // Map Identity and other Razor Pages.
            });
        }
    }
}


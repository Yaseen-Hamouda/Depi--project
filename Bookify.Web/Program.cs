using Bookify.Core.Interfaces;
using Bookify.Core.Models;
using Bookify.Data.Data;
using Bookify.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ==========================================================
            // 1) Database Connection
            // ==========================================================
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                ));

            // ==========================================================
            // 2) Identity (Users + Roles)
            // ==========================================================
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();   // ? ???? ????? Login/Register ????????

            // ==========================================================
            // 3) Dependency Injection (Only UnitOfWork)
            // ==========================================================
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ?? ????? ????? Repositories ??????
            // ??? UnitOfWork ?? ???? ??????? ??? ???? DbContext

            // ==========================================================
            // 4) MVC + Razor Pages
            // ==========================================================
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // ==========================================================
            // 5) Seed Admin Role + Admin User
            // ==========================================================
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Create Admin Role
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));

                // Create Admin User
                string adminEmail = "admin@bookify.com";
                string adminPassword = "Admin123";

                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName = "System Admin",
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(adminUser, adminPassword);
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // ==========================================================
            // 6) HTTP Pipeline
            // ==========================================================
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Authentication BEFORE Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // ==========================================================
            // 7) MVC Routing + Razor Pages
            // ==========================================================
            app.MapAreaControllerRoute(
            name: "AdminArea",
            areaName: "Admin",
            pattern: "Admin/{controller=RoomTypes}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "clientBooking",
                pattern: "Booking/{action=Create}/{roomId?}",
                defaults: new { controller = "Booking" });


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");



            app.MapRazorPages();   // ? Enables /Account/Login

            // ==========================================================
            await app.RunAsync();
        }
    }
}

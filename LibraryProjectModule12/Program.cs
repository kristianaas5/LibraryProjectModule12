using LibraryProjectModule12.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LibraryProjectModule12.Models;
namespace LibraryProjectModule12
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
                {
                    // НАСТРОЙКИ ЗА ПАРОЛАТА
                    options.Password.RequireDigit = true;       
                    options.Password.RequireLowercase = true;       
                    options.Password.RequireUppercase = true;       
                    options.Password.RequireNonAlphanumeric = true; 
                    options.Password.RequiredLength = 3;             // Минимум 3 символа

                    // НАСТРОЙКИ ЗА ПОТРЕБИТЕЛЯ
                    options.User.RequireUniqueEmail = true;  // Email трябва да е уникален

                    // НАСТРОЙКИ ЗА LOCKOUT (ЗАКЛЮЧВАНЕ)
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;  // След 5 грешки -> lockout
                    options.Lockout.AllowedForNewUsers = true;

                    // НАСТРОЙКИ ЗА SIGN IN
                    options.SignIn.RequireConfirmedEmail = false;  // НЕ изискваме потвърден email
                })
                .AddRoles<IdentityRole>()  // КРИТИЧНО! Добавяме ролева поддръжка
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddControllersWithViews();

            // === COOKIE КОНФИГУРАЦИЯ ===
            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Път за Login страница
                options.LoginPath = "/Account/Login";

                // Път за Logout
                options.LogoutPath = "/Account/Logout";

                // Път за AccessDenied (когато потребител няма права)
                options.AccessDeniedPath = "/Account/AccessDenied";

                // Cookie настройки
                options.Cookie.HttpOnly = true;  // Защита срещу XSS атаки
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Само през HTTPS
                options.Cookie.SameSite = SameSiteMode.Strict;  // CSRF защита

                // Време на валидност
                options.ExpireTimeSpan = TimeSpan.FromHours(2);  // Cookie валиден 2 часа
                options.SlidingExpiration = true;  // Подновява се при активност
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            // SEED ДАННИ - Създаване на роли и admin потребител чрез DI от клача SeedData
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    await SeedData.Initialize(services, userManager, roleManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Грешка при seed на данните");
                }
            }

            app.Run();
        }
        public static class SeedData
        {
            public static async Task Initialize(
                IServiceProvider serviceProvider,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager)
            {
                // =================
                // СЪЗДАВАНЕ НА РОЛИ
                // =================
                string[] roleNames = {"Admin", "User"};

                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                        Console.WriteLine($"✓ Роля '{roleName}' създадена");
                    }
                }

                // =============================
                // СЪЗДАВАНЕ НА ADMIN ПОТРЕБИТЕЛ
                // =============================
                var adminEmail = "admin@eventures.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    var admin = new ApplicationUser
                    {
                        UserName = "Admin",
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(admin, "Admin123.");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");

                        Console.WriteLine(new string('=', 24));
                        Console.WriteLine(" ADMIN ПОТРЕБИТЕЛ СЪЗДАДЕН!");
                        Console.WriteLine($"  Email:    {adminEmail}");
                        Console.WriteLine("  Username: Admin");
                        Console.WriteLine("  Password: admin123");
                        Console.WriteLine(new string('=', 24));
                    }
                }
            }
        }
    }
}

using LibraryProjectModule12.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LibraryProjectModule12.Models;
namespace LibraryProjectModule12
{
    public class Program
    {
        /// <summary>
        /// The entry point of the application. Configures and starts the web application, including services,
        /// middleware, and routing.
        /// </summary>
        /// <remarks>This method initializes the application by setting up the dependency injection
        /// container, configuring authentication,  authorization, and cookie settings, and defining the HTTP request
        /// pipeline. It also seeds initial data into the database  using the <see cref="SeedData"/> and <see
        /// cref="DbSeeder"/> classes.  The method is asynchronous to support database seeding and other initialization
        /// tasks that require asynchronous operations.</remarks>
        /// <param name="args">Command-line arguments passed to the application.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException"></exception>
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
                    // Standard password settings   
                    options.Password.RequireDigit = true;       //Enquires at least one digit (0-9) in the password
                    options.Password.RequireLowercase = true;       //Enquires at least one lowercase letter (a-z) in the password
                    options.Password.RequireUppercase = true;       //Enquires at least one uppercase letter (A-Z) in the password
                    options.Password.RequireNonAlphanumeric = true; //Enquires at least one non-alphanumeric character (e.g., !, @, #, etc.) in the password
                    options.Password.RequiredLength = 3;            // Minimum length of the password (in this case, 3 characters)

                    // Standard user settings
                    options.User.RequireUniqueEmail = true;  // Email must be unique for each user

                    // Standard lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Lockout duration of 5 minutes
                    options.Lockout.MaxFailedAccessAttempts = 5;  // After 5 failed attempts, the user will be locked out
                    options.Lockout.AllowedForNewUsers = true; // Lockout is enabled for new users

                    // SignIn settings
                    options.SignIn.RequireConfirmedEmail = false;  // Does not require email confirmation for sign-in
                })
                .AddRoles<IdentityRole>()  // Add support for roles in Identity
                .AddEntityFrameworkStores<ApplicationDbContext>() // Use Entity Framework Core for storing Identity data
                .AddDefaultTokenProviders();// Add default token providers for password reset, email confirmation, etc.

            builder.Services.AddControllersWithViews(); // Add support for MVC controllers and views

            // Set up cookie settings for authentication
            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Path for Login
                options.LoginPath = "/Account/Login";

                // Path за Logout
                options.LogoutPath = "/Account/Logout";

                // Path for AccessDenied (when user tries to access a resource they don't have permission for)
                options.AccessDeniedPath = "/Account/AccessDenied";

                // Cookie settings for security
                options.Cookie.HttpOnly = true;  // Secure cookie, not accessible via JavaScript
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Only sent over HTTPS
                options.Cookie.SameSite = SameSiteMode.Strict;  // CSRF security

                // Time settings for cookie expiration and sliding expiration
                options.ExpireTimeSpan = TimeSpan.FromHours(2);  // Cookie is valid for 2 hours
                options.SlidingExpiration = true;  // If the user is active, the expiration time will be reset with each request
            });

            var app = builder.Build();// Build the application

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
            app.UseStaticFiles(); // Enable serving static files (e.g., CSS, JavaScript, images)
            app.UseHttpsRedirection();// Redirect HTTP requests to HTTPS for security
            app.UseRouting();// Enable routing for the application

            app.UseAuthentication();// Enable authentication middleware to handle user authentication
            app.UseAuthorization();// Enable authorization middleware to handle user authorization


            app.MapStaticAssets();// Map static assets to be served from the wwwroot folder
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();// Map the default controller route and include static assets in the routing configuration
            app.MapRazorPages()
               .WithStaticAssets();// Map Razor Pages and include static assets in the routing configuration

            // Seed initial data into the database, including roles and an admin user, as well as authors and genres
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();// Get the UserManager service to manage user accounts
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();// Get the RoleManager service to manage roles

                    await SeedData.Initialize(services, userManager, roleManager);// Seed initial data for roles and an admin user
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Грешка при seed на данните");// Log any exceptions that occur during the seeding process
                }
            }

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;// Get the ApplicationDbContext service to interact with the database
                var context = services.GetRequiredService<ApplicationDbContext>();// Seed initial data for authors and genres in the database

                await DbSeeder.SeedAuthorsAsync(context);// Seed initial data for authors in the database
                await DbSeeder.SeedGenresAsync(context);// Seed initial data for genres in the database
                await DbSeeder.SeedBooksAsync(context);// Seed initial data for books in the database
                await DbSeeder.SeedEventsAsync(context);// Seed initial data for events in the database
            }

            app.Run();// Start the application and listen for incoming HTTP requests
        }
        public static class SeedData
        {
            /// <summary>
            /// Initializes the application by creating predefined roles and an administrator account if they do not
            /// already exist.
            /// </summary>
            /// <remarks>This method ensures that the roles "Admin" and "User" are created in the
            /// system if they do not already exist. Additionally, it creates an administrator account with the email
            /// "admin@eventures.com" and assigns it to the "Admin" role if such an account does not already exist. The
            /// administrator account is created with the username "Admin" and the default password
            /// "Admin123.".</remarks>
            /// <param name="serviceProvider">The service provider used to resolve dependencies.</param>
            /// <param name="userManager">The <see cref="UserManager{TUser}"/> instance used to manage user accounts.</param>
            /// <param name="roleManager">The <see cref="RoleManager{TRole}"/> instance used to manage roles.</param>
            /// <returns></returns>
            public static async Task Initialize(
                IServiceProvider serviceProvider,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager)
            {
                string[] roleNames = {"Admin", "User"};
                // Create roles if they do not exist
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                        Console.WriteLine($"✓ Роля '{roleName}' създадена");
                    }
                }

                //Create admin user
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

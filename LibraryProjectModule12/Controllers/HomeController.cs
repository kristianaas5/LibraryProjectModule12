using System.Diagnostics;
using LibraryProjectModule12.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryProjectModule12.Controllers
{
    /// <summary>
    /// HomeController is responsible for handling requests to the home page of the application. It determines which view to display based on the user's authentication status and role. If the user is not authenticated, it shows a guest index view. If the user is authenticated and has the "Admin" role, it shows an admin index view. For all other authenticated users, it shows a user index view. Additionally, it includes an error action method that returns an error view when an error occurs.
    /// </summary>
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Check if the user is not authenticated. If they are not, return the "GuestIndex" view.
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return View("GuestIndex");
            }
            // If the user is authenticated, check if they have the "Admin" role. If they do, return the "AdminIndex" view.
            if (User.IsInRole("Admin"))
            {

                return View("AdminIndex");
            }
            // For all other authenticated users who do not have the "Admin" role, return the "UserIndex" view.
            return View("UserIndex");
        }
        // The Error action method is decorated with the ResponseCache attribute to prevent caching of the error response. It returns the "Error" view when an error occurs in the application, allowing users to see a friendly error message instead of a generic server error page.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}

using System.Diagnostics;
using LibraryProjectModule12.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryProjectModule12.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // ПРОВЕРКА: Потребителят влязъл ли е?
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                // Невлезли потребители (Guests)
                return View("GuestIndex");
            }

            // ПРОВЕРКА: Потребителят Admin ли е?
            if (User.IsInRole("Admin"))
            {
                // Admin потребители
                return View("AdminIndex");
            }

            // Обикновени потребители (User роля)
            return View("UserIndex");
        }

        /// <summary>
        /// GET: /Home/Error
        /// Страница за грешки
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}

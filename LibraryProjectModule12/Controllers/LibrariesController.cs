using LibraryProjectModule12.Data;
using LibraryProjectModule12.Models;
using LibraryProjectModule12.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryProjectModule12.Controllers
{
    [Authorize] // require login to see personal shelves
    public class LibrariesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LibrariesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous] // or remove to require login for all
        public async Task<IActionResult> Index()
        {
            var baseQuery = _context.Libraries
                .Include(l => l.User)
                .Include(l => l.Book).ThenInclude(b => b.Author)
                .Include(l => l.Book).ThenInclude(b => b.Genre)
                .Include(l => l.Reviews);

            if (User.IsInRole("Admin"))
            {
                // Admin sees all entries in a simple list
                var items = await baseQuery
                    .OrderBy(l => l.User!.UserName)
                    .ThenBy(l => l.Book!.Name)
                    .ToListAsync();

                return View("AdminIndex", items); // Views/Libraries/AdminIndex.cshtml
            }

            // Regular user: show only their shelves grouped
            IQueryable<Library> query = baseQuery;
            var currentUserId = _context.Users
                .Where(u => u.UserName == User.Identity!.Name)
                .Select(u => u.Id)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(currentUserId))
            {
                query = query.Where(l => l.UserId == currentUserId);
            }

            var userItems = await query.ToListAsync();

            var vm = new LibraryShelvesViewModel
            {
                WantToRead = userItems.Where(l => l.ShelfType == ShelfType.WantToRead).OrderBy(l => l.Book!.Name).ToList(),
                CurrentlyReading = userItems.Where(l => l.ShelfType == ShelfType.CurrentlyReading).OrderBy(l => l.Book!.Name).ToList(),
                ReadReviewed = userItems.Where(l => l.ShelfType == ShelfType.Read && (l.Reviews?.Any() ?? false)).OrderBy(l => l.Book!.Name).ToList(),
                ReadNotReviewed = userItems.Where(l => l.ShelfType == ShelfType.Read && !(l.Reviews?.Any() ?? false)).OrderBy(l => l.Book!.Name).ToList()
            };

            return View(vm); // Views/Libraries/Index.cshtml (current grouped shelves)
        }

        // POST: /Libraries/AddFromBook/5?shelfType=WantToRead
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFromBook(int bookId, ShelfType shelfType = ShelfType.WantToRead)
        {
            // Resolve current user
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized();

            var userId = await _context.Users
                .Where(u => u.UserName == userName)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Validate book exists
            var bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExists)
            {
                TempData["Error"] = "Selected book does not exist.";
                return RedirectToAction("Index", "Books");
            }

            // Prevent duplicates for the same user/book
            var alreadyExists = await _context.Libraries
                .AnyAsync(l => l.UserId == userId && l.BookId == bookId && !l.IsDeleted);
            if (alreadyExists)
            {
                TempData["Info"] = "This book is already in your library.";
                return RedirectToAction("Details", "Books", new { id = bookId });
            }

            // Create entry
            var entry = new Library
            {
                UserId = userId,
                BookId = bookId,
                ShelfType = shelfType,
                IsDeleted = false
            };

            _context.Libraries.Add(entry);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Book added to your library.";
            return RedirectToAction("Index", "Libraries");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeShelf(int id, ShelfType shelfType)
        {
            var entry = await _context.Libraries
                .Include(l => l.Book)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (entry == null)
            {
                TempData["Error"] = "Library entry not found.";
                return RedirectToAction(nameof(Index));
            }

            entry.ShelfType = shelfType;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Shelf updated to {shelfType}.";
            // Return to Admin view
            return RedirectToAction(nameof(Index));
        }
    }
}
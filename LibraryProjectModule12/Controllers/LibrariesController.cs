using LibraryProjectModule12.Data;
using LibraryProjectModule12.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryProjectModule12.Controllers
{
    // Controller for managing Library entries (user shelves) with CRUD
    // Uses soft-delete via IsDeleted and relies on global query filters to hide deleted rows.
    [Authorize(Roles = "Admin")]
    public class LibrariesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LibrariesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper: populate dropdowns for User, Book, ShelfType
        private async Task PopulateSelectionsAsync(string? selectedUserId = null, int? selectedBookId = null, ShelfType? selectedShelf = null)
        {
            var users = await _context.Users
                .OrderBy(u => u.UserName)
                .Select(u => new { u.Id, Display = string.IsNullOrEmpty(u.UserName) ? u.Email : u.UserName })
                .ToListAsync();

            var books = await _context.Books
                .OrderBy(b => b.Name)
                .Select(b => new { b.Id, Display = $"{b.Name} ({b.Year})" })
                .ToListAsync();

            ViewData["UserId"] = new SelectList(users, "Id", "Display", selectedUserId);
            ViewData["BookId"] = new SelectList(books, "Id", "Display", selectedBookId);

            var shelfItems = Enum.GetValues(typeof(ShelfType))
                .Cast<ShelfType>()
                .Select(s => new { Id = s, Name = s.ToString() })
                .ToList();

            ViewData["ShelfType"] = new SelectList(shelfItems, "Id", "Name", selectedShelf);
        }

        // GET: /Libraries
        // List all non-deleted library entries with related user and book
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var items = await _context.Libraries
                .Include(l => l.User)
                .Include(l => l.Book)
                .OrderBy(l => l.User!.UserName)
                .ThenBy(l => l.Book!.Name)
                .ToListAsync();

            return View(items);
        }

        // GET: /Libraries/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var library = await _context.Libraries
                .Include(l => l.User)
                .Include(l => l.Book)
                .Include(l => l.Reviews)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (library == null) return NotFound();
            return View(library);
        }

        // GET: /Libraries/Create
        public async Task<IActionResult> Create()
        {
            await PopulateSelectionsAsync();
            return View(new Library());
        }

        // POST: /Libraries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Library model)
        {
            // Basic validation: ensure referenced User and Book exist
            if (!await _context.Users.AnyAsync(u => u.Id == model.UserId))
                ModelState.AddModelError(nameof(model.UserId), "Selected user does not exist.");

            if (!await _context.Books.AnyAsync(b => b.Id == model.BookId))
                ModelState.AddModelError(nameof(model.BookId), "Selected book does not exist.");

            if (!ModelState.IsValid)
            {
                await PopulateSelectionsAsync(model.UserId, model.BookId, model.ShelfType);
                return View(model);
            }

            _context.Libraries.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Libraries/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var library = await _context.Libraries.FindAsync(id);
            if (library == null) return NotFound();

            await PopulateSelectionsAsync(library.UserId, library.BookId, library.ShelfType);
            return View(library);
        }

        // POST: /Libraries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Library model)
        {
            if (id != model.Id) return BadRequest();

            if (!await _context.Users.AnyAsync(u => u.Id == model.UserId))
                ModelState.AddModelError(nameof(model.UserId), "Selected user does not exist.");

            if (!await _context.Books.AnyAsync(b => b.Id == model.BookId))
                ModelState.AddModelError(nameof(model.BookId), "Selected book does not exist.");

            if (!ModelState.IsValid)
            {
                await PopulateSelectionsAsync(model.UserId, model.BookId, model.ShelfType);
                return View(model);
            }

            try
            {
                _context.Entry(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.Libraries.AnyAsync(l => l.Id == id);
                if (!exists) return NotFound();
                throw;
            }
        }

        // GET: /Libraries/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var library = await _context.Libraries
                .Include(l => l.User)
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (library == null) return NotFound();
            return View(library);
        }

        // POST: /Libraries/Delete/5
        // Soft-delete: sets IsDeleted = true
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var library = await _context.Libraries.FirstOrDefaultAsync(l => l.Id == id);
            if (library == null) return NotFound();

            library.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Optional: Restore soft-deleted entry
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var library = await _context.Libraries
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(l => l.Id == id);

            if (library == null) return NotFound();

            library.IsDeleted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
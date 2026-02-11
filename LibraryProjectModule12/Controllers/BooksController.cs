using LibraryProjectModule12.Data;
using LibraryProjectModule12.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryProjectModule12.Controllers
{
    [Authorize] // Require authentication for non-read actions; Index/Details are allowed anonymously below.
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper: populate dropdowns for Author and Genre
        private async Task PopulateSelectionsAsync(int? selectedAuthorId = null, int? selectedGenreId = null)
        {
            var authors = await _context.Authors
                .OrderBy(a => a.LastName).ThenBy(a => a.Name)
                .Select(a => new { a.Id, Display = $"{a.LastName}, {a.Name}" })
                .ToListAsync();

            var genres = await _context.Genres
                .OrderBy(g => g.Name)
                .Select(g => new { g.Id, g.Name })
                .ToListAsync();

            ViewData["AuthorId"] = new SelectList(authors, "Id", "Display", selectedAuthorId);
            ViewData["GenreId"] = new SelectList(genres, "Id", "Name", selectedGenreId);
        }

        // GET: /Books
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .OrderBy(b => b.Name)
                .ThenBy(b => b.Year)
                .ToListAsync();

            return View(books);
        }

        // GET: /Books/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();
            return View(book);
        }

        // GET: /Books/Create
        // Regular authenticated users can add books (no Admin requirement).
        public async Task<IActionResult> Create()
        {
            await PopulateSelectionsAsync();
            return View(new Book());
        }

        // POST: /Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book model)
        {
            // Validate foreign keys exist
            if (!await _context.Authors.AnyAsync(a => a.Id == model.AuthorId))
                ModelState.AddModelError(nameof(model.AuthorId), "Selected author does not exist.");
            if (!await _context.Genres.AnyAsync(g => g.Id == model.GenreId))
                ModelState.AddModelError(nameof(model.GenreId), "Selected genre does not exist.");

            if (!ModelState.IsValid)
            {
                await PopulateSelectionsAsync(model.AuthorId, model.GenreId);
                return View(model);
            }

            _context.Books.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Books/Edit/5
        // Regular users cannot edit; only Admins.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            await PopulateSelectionsAsync(book.AuthorId, book.GenreId);
            return View(book);
        }

        // POST: /Books/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book model)
        {
            if (id != model.Id) return BadRequest();

            if (!await _context.Authors.AnyAsync(a => a.Id == model.AuthorId))
                ModelState.AddModelError(nameof(model.AuthorId), "Selected author does not exist.");
            if (!await _context.Genres.AnyAsync(g => g.Id == model.GenreId))
                ModelState.AddModelError(nameof(model.GenreId), "Selected genre does not exist.");

            if (!ModelState.IsValid)
            {
                await PopulateSelectionsAsync(model.AuthorId, model.GenreId);
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
                var exists = await _context.Books.AnyAsync(b => b.Id == id);
                if (!exists) return NotFound();
                throw;
            }
        }

        // GET: /Books/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();
            return View(book);
        }

        // POST: /Books/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) return NotFound();

            book.IsDeleted = true; // soft-delete
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Optional: Restore soft-deleted book, Admin only.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var book = await _context.Books.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) return NotFound();

            book.IsDeleted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
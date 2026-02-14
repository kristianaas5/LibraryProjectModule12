using LibraryProjectModule12.Data;
using LibraryProjectModule12.Models;
using LibraryProjectModule12.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryProjectModule12.Controllers
{
    // Controller for managing Authors with full CRUD (Create, Read, Update, Delete)
    // Uses soft-delete via IsDeleted and excludes deleted rows via ApplicationDbContext query filters.
    [Authorize(Roles = "Admin")]
    public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Authors
        // List all non-deleted authors
        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            var query = _context.Authors
                .OrderBy(a => a.LastName).ThenBy(a => a.Name);

            var items = await query.ToListAsync(); // or take page/pageSize if needed
            return View(items); // view expects IEnumerable<Author>
        }
        // GET: /Authors/Deleted
        public async Task<IActionResult> IndexDelete(string query)
        {
            var authorsQuery = _context.Authors
                .IgnoreQueryFilters() // Include deleted authors
                .Where(a => a.IsDeleted) // Only deleted authors
                .OrderBy(a => a.LastName).ThenBy(a => a.Name);
            var authors = await authorsQuery.ToListAsync();
            return View(authors);
        }


        // GET: /Authors/Details/5
        // Show one author by Id
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);
            // If the author is not found (either doesn't exist or is deleted), return 404 Not Found.
            if (author == null) return NotFound();
            return View(author);
        }

        // GET: /Authors/Create
        // Render create form
        public IActionResult Create()
        {
            return View(new Author());
        }

        // POST: /Authors/Create
        // Create an author
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author model)
        {
            if (!ModelState.IsValid) return View(model);

            _context.Authors.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Authors/Edit/5
        // Render edit form
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _context.Authors.FindAsync(id);// Query filters exclude deleted, so this will return null if the author is deleted.
            if (author == null) return NotFound();// If the author is not found (either doesn't exist or is deleted), return 404 Not Found.
            return View(author);
        }

        // POST: /Authors/Edit/5
        // Update an author
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Author model)
        {
            if (id != model.Id) return BadRequest();// Id in URL must match Id in form data
            if (!ModelState.IsValid) return View(model);// If the model state is invalid, return the view with the model to display validation errors.
            
            try
            {
                _context.Entry(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Authors.AnyAsync(a => a.Id == id))
                    return NotFound();
                throw;
            }
        }

        // GET: /Authors/Delete/5
        // Confirm delete (soft-delete)
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id);// Query filters exclude deleted, so this will return null if the author is deleted.
            if (author == null) return NotFound();// If the author is not found (either doesn't exist or is deleted), return 404 Not Found.
            return View(author);
        }

        // POST: /Authors/Delete/5
        // Soft-delete author (sets IsDeleted = true)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Query filters exclude deleted; this will return null if the author is already deleted.
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id);
            if (author == null) return NotFound();
            // Soft-delete by setting IsDeleted to true. The author will be excluded from normal queries due to the global query filter.
            author.IsDeleted = true;
            await _context.SaveChangesAsync();
            // Optionally, you could store the deleted author's ID in TempData to allow for an "Undo" feature.
            TempData["Success"] = "Author deleted.";
            TempData["UndoAuthorId"] = author.Id;

            return RedirectToAction(nameof(Index));
        }
        // GET: /Authors/Restore/5
        // Render restore confirmation for a deleted author
        public async Task<IActionResult> Restore(int id)
        {
            var author = await _context.Authors.IgnoreQueryFilters() // Include deleted authors
                .Where(a => a.IsDeleted).FirstOrDefaultAsync(a => a.Id == id);
            if (author == null) return NotFound();// If the author is not found (either doesn't exist or is not deleted), return 404 Not Found.
            return View(author);
        }

        //Optional: Undelete
        //POST: /Authors/Restore/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Author model)
        {
            // Query filters exclude deleted; use IgnoreQueryFilters to find it.
            var author = await _context.Authors.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Id == model.Id);
            if (author == null) return NotFound();

            author.IsDeleted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { model.Id });
        }
    }
}
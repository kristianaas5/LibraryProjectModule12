using LibraryProjectModule12.Data;
using LibraryProjectModule12.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryProjectModule12.Controllers
{
    public class GenresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: /Genres/Index
        // This action retrieves a list of all genres that are not marked as deleted (where IsDeleted is false) and displays them in a view. It orders the genres alphabetically by name for better user experience.
        public async Task<IActionResult> Index()
        {
            var genres = await _context.Genres
                .OrderBy(g => g.Name)
                .ToListAsync();
            return View(genres);
        }
        [Authorize(Roles = "Admin")]
        // GET: /Genres/IndexDelete
        // This action retrieves a list of soft-deleted genres (where IsDeleted is true) and displays them in a view. It uses IgnoreQueryFilters() to include deleted records in the query results, allowing administrators to see and manage deleted genres.
        public async Task<IActionResult> IndexDelete()
        {
            var deletedGenres = await _context.Genres
                .IgnoreQueryFilters()
                .Where(g => g.IsDeleted)
                .OrderBy(g => g.Name)
                .ToListAsync();
            return View(deletedGenres);
        }

        // GET: /Genres/Details/5
        [AllowAnonymous]
        // This action retrieves the details of a specific genre by its ID, including the related books. If the genre is not found, it returns a 404 Not Found response. Otherwise, it displays the genre details in a view.
        public async Task<IActionResult> Details(int id)
        {
            var genre = await _context.Genres
                .Include(g => g.Books)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (genre == null) return NotFound();
            return View(genre);
        }

        // GET: /Genres/Create
        [Authorize(Roles = "Admin")]
        // This action returns a view for creating a new genre. It initializes a new Genre object and passes it to the view, allowing administrators to fill in the details for the new genre.
        public IActionResult Create()
        {
            return View(new Genre());
        }

        // POST: /Genres/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // This action handles the form submission for creating a new genre. It checks if the model state is valid, and if so, it adds the new genre to the database context and saves the changes. After successfully creating the genre, it redirects to the Index action to display the list of genres.
        public async Task<IActionResult> Create(Genre model)
        {
            // Validation: Check if the model state is valid. If it is not, return the view with the model to display validation errors.
            if (!ModelState.IsValid) return View(model);
            // Add the new genre to the database context and save changes.
            _context.Genres.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Genres/Edit/5
        [Authorize(Roles = "Admin")]
        // This action retrieves a specific genre by its ID for editing. If the genre is not found, it returns a 404 Not Found response. Otherwise, it returns a view with the genre details pre-filled for editing.
        public async Task<IActionResult> Edit(int id)
        {
            // Retrieve the genre by its ID. If it does not exist, return NotFound.
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound();// Return the view with the genre details for editing.
            return View(genre);
        }

        // POST: /Genres/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // This action handles the form submission for editing an existing genre. It checks if the provided ID matches the genre's ID and if the model state is valid. If valid, it updates the genre in the database context and saves the changes. If a concurrency exception occurs (e.g., another user has modified the genre), it checks if the genre still exists and either returns NotFound or rethrows the exception.
        public async Task<IActionResult> Edit(int id, Genre model)
        {
            // Check if the provided ID matches the genre's ID. If not, return BadRequest.
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);// If the model state is invalid, return the view with the model to display validation errors.
            // Update the genre in the database context and save changes. Handle potential concurrency exceptions.
            try
            {
                _context.Entry(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }// If a concurrency exception occurs, check if the genre still exists. If it does not exist, return NotFound. Otherwise, rethrow the exception.
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.Genres.AnyAsync(g => g.Id == id);
                if (!exists) return NotFound();
                throw;
            }
        }

        // GET: /Genres/Delete/5
        [Authorize(Roles = "Admin")]
        // This action retrieves a specific genre by its ID for deletion. If the genre is not found, it returns a 404 Not Found response. Otherwise, it returns a view with the genre details, allowing administrators to confirm the deletion.
        public async Task<IActionResult> Delete(int id)
        {
            // Retrieve the genre by its ID. If it does not exist, return NotFound. Otherwise, return the view with the genre details for deletion confirmation.
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);// Return the view with the genre details for deletion confirmation.
            if (genre == null) return NotFound();// Return the view with the genre details for deletion confirmation.
            return View(genre);
        }

        // POST: /Genres/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //  This action handles the form submission for deleting a genre. Instead of permanently removing the genre from the database, it marks the genre as deleted by setting the IsDeleted property to true. It then saves the changes to the database and redirects to the Index action. Additionally, it uses TempData to store a success message and the ID of the deleted genre, allowing for an undo option in the user interface.
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //  Retrieve the genre by its ID. If it does not exist, return NotFound. Otherwise, mark the genre as deleted by setting IsDeleted to true and save changes to the database.
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);
            if (genre == null) return NotFound();// Mark the genre as deleted and save changes to the database. Optionally, store the deleted genre's ID in TempData to allow for an "Undo" feature.

            genre.IsDeleted = true;
            await _context.SaveChangesAsync();// Optionally, store the deleted genre's ID in TempData to allow for an "Undo" feature.

            TempData["Success"] = "Genre deleted.";
            TempData["UndoGenreId"] = id;

            return RedirectToAction(nameof(Index));
        }
        // GET: /Genres/Restore/5
        // This action retrieves a specific soft-deleted genre by its ID for restoration. If the genre is not found, it returns a 404 Not Found response. Otherwise, it returns a view with the genre details, allowing administrators to confirm the restoration of the genre.
        public async Task<IActionResult> Restore(int id)
        {
            var genre = await _context.Genres.IgnoreQueryFilters() // Include deleted authors
                .Where(a => a.IsDeleted).FirstOrDefaultAsync(g => g.Id == id);
            if (genre == null) return NotFound();
            return View(genre);
        }

        // Optional: Restore soft-deleted genre
        // POST: /Genres/Restore/5
        // This action handles the form submission for restoring a soft-deleted genre. It retrieves the genre by its ID, checks if it exists, and if so, it marks the genre as not deleted by setting the IsDeleted property to false. It then saves the changes to the database and redirects to the Index action.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Genre model)
        {
            // Retrieve the genre by its ID, including soft-deleted records. If it does not exist, return NotFound. Otherwise, mark the genre as not deleted and save changes to the database.
            var genre = await _context.Genres.IgnoreQueryFilters().FirstOrDefaultAsync(g => g.Id == model.Id);// Mark the genre as not deleted and save changes to the database. If the genre does not exist, return NotFound.
            if (genre == null) return NotFound();// Mark the genre as not deleted and save changes to the database.
            // Mark the genre as not deleted and save changes to the database.
            genre.IsDeleted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { model.Id });
        }
    }
}
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

        // GET: /Genres
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var genres = await _context.Genres
                .OrderBy(g => g.Name)
                .ToListAsync();
            return View(genres);
        }

        // GET: /Genres/Details/5
        [AllowAnonymous]
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
        public IActionResult Create()
        {
            return View(new Genre());
        }

        // POST: /Genres/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Genre model)
        {
            if (!ModelState.IsValid) return View(model);

            _context.Genres.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Genres/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound();
            return View(genre);
        }

        // POST: /Genres/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Genre model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            try
            {
                _context.Entry(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.Genres.AnyAsync(g => g.Id == id);
                if (!exists) return NotFound();
                throw;
            }
        }

        // GET: /Genres/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);
            if (genre == null) return NotFound();
            return View(genre);
        }

        // POST: /Genres/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);
            if (genre == null) return NotFound();

            genre.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Optional: Restore soft-deleted genre
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var genre = await _context.Genres.IgnoreQueryFilters().FirstOrDefaultAsync(g => g.Id == id);
            if (genre == null) return NotFound();

            genre.IsDeleted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
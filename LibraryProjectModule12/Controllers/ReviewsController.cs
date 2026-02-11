using LibraryProjectModule12.Data;
using LibraryProjectModule12.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryProjectModule12.Controllers
{
    [Authorize] // require login to add reviews
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Reviews/Create?libraryId=123
        public async Task<IActionResult> Create(int libraryId)
        {
            var library = await _context.Libraries
                .Include(l => l.Book)!.ThenInclude(b => b.Author)
                .FirstOrDefaultAsync(l => l.Id == libraryId);

            if (library == null)
            {
                TempData["Error"] = "Library entry not found.";
                return RedirectToAction("Index", "Libraries");
            }

            if (library.ShelfType != ShelfType.Read)
            {
                TempData["Error"] = "Reviews can only be added for books with shelf status 'Read'.";
                return RedirectToAction("Index", "Libraries");
            }

            // Prefill a review model with LibraryId
            var model = new Review
            {
                LibraryId = libraryId
            };

            ViewBag.Library = library;
            return View(model);
        }

        // POST: /Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review model)
        {
            // Ensure Library exists and is 'Read'
            var library = await _context.Libraries
                .Include(l => l.Book)!.ThenInclude(b => b.Author)
                .FirstOrDefaultAsync(l => l.Id == model.LibraryId);

            if (library == null)
            {
                TempData["Error"] = "Library entry not found.";
                return RedirectToAction("Index", "Libraries");
            }

            if (library.ShelfType != ShelfType.Read)
            {
                TempData["Error"] = "Reviews can only be added for books with shelf status 'Read'.";
                return RedirectToAction("Index", "Libraries");
            }

            // Server-side validation for rating range (model has [Range(1,5)])
            if (!ModelState.IsValid)
            {
                ViewBag.Library = library;
                return View(model);
            }

            // Persist review
            _context.Reviews.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Review added.";
            return RedirectToAction("Index", "Libraries");
        }

        // POST: /Reviews/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Library)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null) return NotFound();

            var isOwner = review.Library?.UserId != null && User.Identity?.IsAuthenticated == true &&
                          await _context.Users.AnyAsync(u => u.Id == review.Library!.UserId && u.UserName == User.Identity!.Name);
            if (!isOwner && !User.IsInRole("Admin"))
                return Forbid();

            review.IsDeleted = true;
            await _context.SaveChangesAsync();

            // Provide Undo link
            TempData["Success"] = "Review deleted.";
            TempData["UndoReviewId"] = review.Id;

            return RedirectToAction("Index", "Libraries");
        }

        // Undo soft delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var review = await _context.Reviews
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null) return NotFound();

            // Ownership/Admin check
            var library = await _context.Libraries.IgnoreQueryFilters().FirstOrDefaultAsync(l => l.Id == review.LibraryId);
            var isOwner = library?.UserId != null && User.Identity?.IsAuthenticated == true &&
                          await _context.Users.AnyAsync(u => u.Id == library!.UserId && u.UserName == User.Identity!.Name);
            if (!isOwner && !User.IsInRole("Admin"))
                return Forbid();

            review.IsDeleted = false;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Review restored.";
            return RedirectToAction("Index", "Libraries");
        }
    }
}
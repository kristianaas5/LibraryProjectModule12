using LibraryProjectModule12.Controllers;
using LibraryProjectModule12.Data;
using LibraryProjectModule12.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LIbraryProjectModule12NUnitTests
{
    public class ReviewsControllerTests
    {
        private ApplicationDbContext _context;
        private ReviewsController _controller;

        private const string OwnerUserId = "owner-user-id";
        private const string OtherUserId = "other-user-id";
        private const string OwnerUserName = "owner@example.com";
        private const string OtherUserName = "other@example.com";
        private const string AdminUserId = "admin-user-id";
        private const string AdminUserName = "admin@example.com";

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            _controller = new ReviewsController(_context);
            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                new TempDataProvider());

            SeedData();

            // Default: authenticated as non-owner, non-admin
            SetUserPrincipal(OtherUserId, OtherUserName, isAuthenticated: true, isAdmin: false);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
            _controller.Dispose();
        }

        private void SeedData()
        {
            // ApplicationUsers
            _context.Users.Add(new ApplicationUser
            {
                Id = OwnerUserId,
                UserName = OwnerUserName,
                Email = OwnerUserName,
                Name = "Owner",
                LastName = "User",
                DateRegistrated = DateTime.UtcNow,
                IsDeleted = false
            });
            _context.Users.Add(new ApplicationUser
            {
                Id = OtherUserId,
                UserName = OtherUserName,
                Email = OtherUserName,
                Name = "Other",
                LastName = "User",
                DateRegistrated = DateTime.UtcNow,
                IsDeleted = false
            });
            _context.Users.Add(new ApplicationUser
            {
                Id = AdminUserId,
                UserName = AdminUserName,
                Email = AdminUserName,
                Name = "Admin",
                LastName = "User",
                DateRegistrated = DateTime.UtcNow,
                IsDeleted = false
            });

            // Author/Genre/Book for Library
            var author = new Author { Id = 1, Name = "John", LastName = "Doe", Country = "BG", IsDeleted = false };
            var genre = new Genre { Id = 1, Name = "Fiction", Description = "Desc", IsDeleted = false };
            var book = new Book
            {
                Id = 1,
                Name = "Book",
                Year = 2020,
                AuthorId = author.Id,
                GenreId = genre.Id,
                Description = "Desc",
                IsDeleted = false
            };
            _context.Authors.Add(author);
            _context.Genres.Add(genre);
            _context.Books.Add(book);

            // Libraries
            _context.Libraries.Add(new Library
            {
                Id = 1,
                UserId = OwnerUserId, // must match ApplicationUser.Id
                BookId = book.Id,
                ShelfType = ShelfType.Read,
                IsDeleted = false
            });
            _context.Libraries.Add(new Library
            {
                Id = 2,
                UserId = OwnerUserId,
                BookId = book.Id,
                ShelfType = ShelfType.WantToRead,
                IsDeleted = false
            });

            // Existing review in LibraryId=1
            _context.Reviews.Add(new Review
            {
                Id = 1,
                LibraryId = 1,
                Rating = 4,
                ReviewText = "Nice book",
                IsDeleted = false
            });

            _context.SaveChanges();
        }

        private void SetUserPrincipal(string userId, string userName, bool isAuthenticated, bool isAdmin)
        {
            var identity = new ClaimsIdentity();
            if (isAuthenticated)
            {
                identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userName),           // ReviewsController checks User.Identity.Name against ApplicationUser.UserName
                    new Claim(ClaimTypes.NameIdentifier, userId)     // typical NameIdentifier is the user Id
                }, "TestAuthType");

                if (isAdmin)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
                }
            }

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Test]
        public async Task Create_Get_Redirects_WhenLibraryMissing()
        {
            var result = await _controller.Create(999) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ControllerName, Is.EqualTo("Libraries"));
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(_controller.TempData.ContainsKey("Error"), Is.True);
        }

        [Test]
        public async Task Create_Get_Redirects_WhenShelfNotRead()
        {
            var result = await _controller.Create(2) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ControllerName, Is.EqualTo("Libraries"));
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(_controller.TempData.ContainsKey("Error"), Is.True);
        }

        [Test]
        public async Task Create_Get_ReturnsView_WithPrefilledModel_WhenLibraryRead()
        {
            var result = await _controller.Create(1) as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as Review;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.LibraryId, Is.EqualTo(1));
            Assert.That(_controller.ViewBag.Library, Is.Not.Null);
        }

        [Test]
        public async Task Create_Post_Redirects_WhenLibraryMissing()
        {
            var result = await _controller.Create(new Review
            {
                LibraryId = 999,
                Rating = 5
            }) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ControllerName, Is.EqualTo("Libraries"));
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(_controller.TempData.ContainsKey("Error"), Is.True);
        }

        [Test]
        public async Task Create_Post_Redirects_WhenShelfNotRead()
        {
            var result = await _controller.Create(new Review
            {
                LibraryId = 2,
                Rating = 5
            }) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ControllerName, Is.EqualTo("Libraries"));
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(_controller.TempData.ContainsKey("Error"), Is.True);
        }

        [Test]
        public async Task Create_Post_InvalidModel_ReturnsView()
        {
            var model = new Review
            {
                LibraryId = 1,
                Rating = 0,
                ReviewText = "Too low"
            };
            _controller.ModelState.AddModelError(nameof(Review.Rating), "Range");

            var result = await _controller.Create(model) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(_controller.ModelState.IsValid, Is.False);
            Assert.That(result.Model, Is.SameAs(model));
            Assert.That(_controller.ViewBag.Library, Is.Not.Null);
        }

        [Test]
        public async Task Create_Post_ValidModel_AddsReview_AndRedirects()
        {
            var model = new Review
            {
                LibraryId = 1,
                Rating = 5,
                ReviewText = "Excellent"
            };

            var result = await _controller.Create(model) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ControllerName, Is.EqualTo("Libraries"));
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(_context.Reviews.Count(), Is.EqualTo(2));
            Assert.That(_controller.TempData.ContainsKey("Success"), Is.True);
        }

        [Test]
        public async Task DeleteConfirmed_ReturnsNotFound_WhenReviewMissing()
        {
            var result = await _controller.DeleteConfirmed(999);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteConfirmed_Forbid_WhenNotOwner_AndNotAdmin()
        {
            // principal is OtherUserName, review belongs to OwnerUserId
            var result = await _controller.DeleteConfirmed(1);

            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        [Test]
        public async Task DeleteConfirmed_SoftDeletes_WhenOwner()
        {
            // switch to owner principal
            SetUserPrincipal(OwnerUserId, OwnerUserName, isAuthenticated: true, isAdmin: false);

            var result = await _controller.DeleteConfirmed(1) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ControllerName, Is.EqualTo("Libraries"));
            Assert.That(result.ActionName, Is.EqualTo("Index"));

            var review = await _context.Reviews.IgnoreQueryFilters().FirstAsync(r => r.Id == 1);
            Assert.That(review.IsDeleted, Is.True);

            Assert.That(_controller.TempData.ContainsKey("Success"), Is.True);
            Assert.That(_controller.TempData.ContainsKey("UndoReviewId"), Is.True);
            Assert.That(_controller.TempData["UndoReviewId"], Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteConfirmed_AllowsAdminEvenIfNotOwner()
        {
            // admin principal
            SetUserPrincipal(AdminUserId, AdminUserName, isAuthenticated: true, isAdmin: true);

            var result = await _controller.DeleteConfirmed(1) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ControllerName, Is.EqualTo("Libraries"));
            Assert.That(result.ActionName, Is.EqualTo("Index"));

            var review = await _context.Reviews.IgnoreQueryFilters().FirstAsync(r => r.Id == 1);
            Assert.That(review.IsDeleted, Is.True);
        }

        [Test]
        public async Task Restore_ReturnsNotFound_WhenReviewMissing()
        {
            var result = await _controller.Restore(999);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Restore_Forbid_WhenNotOwner_AndNotAdmin()
        {
            var review = await _context.Reviews.FirstAsync(r => r.Id == 1);
            review.IsDeleted = true;
            await _context.SaveChangesAsync();

            // principal is OtherUserName (not owner, not admin)
            var result = await _controller.Restore(1);

            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        [Test]
        public async Task Restore_AllowsOwner_AndRestores()
        {
            var review = await _context.Reviews.FirstAsync(r => r.Id == 1);
            review.IsDeleted = true;
            await _context.SaveChangesAsync();

            // switch to owner
            SetUserPrincipal(OwnerUserId, OwnerUserName, isAuthenticated: true, isAdmin: false);

            var result = await _controller.Restore(1) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ControllerName, Is.EqualTo("Libraries"));
            Assert.That(result.ActionName, Is.EqualTo("Index"));

            var updated = await _context.Reviews.IgnoreQueryFilters().FirstAsync(r => r.Id == 1);
            Assert.That(updated.IsDeleted, Is.False);
            Assert.That(_controller.TempData.ContainsKey("Success"), Is.True);
        }

        [Test]
        public async Task Restore_AllowsAdmin_AndRestores()
        {
            var review = await _context.Reviews.FirstAsync(r => r.Id == 1);
            review.IsDeleted = true;
            await _context.SaveChangesAsync();

            // admin principal
            SetUserPrincipal(AdminUserId, AdminUserName, isAuthenticated: true, isAdmin: true);

            var result = await _controller.Restore(1) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ControllerName, Is.EqualTo("Libraries"));
            Assert.That(result.ActionName, Is.EqualTo("Index"));

            var updated = await _context.Reviews.IgnoreQueryFilters().FirstAsync(r => r.Id == 1);
            Assert.That(updated.IsDeleted, Is.False);
        }

        // Minimal ITempDataProvider for TempDataDictionary
        private sealed class TempDataProvider : ITempDataProvider
        {
            public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
            public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
        }
    }
}
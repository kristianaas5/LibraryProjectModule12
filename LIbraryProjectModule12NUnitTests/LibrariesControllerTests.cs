using LibraryProjectModule12.Controllers;
using LibraryProjectModule12.Data;
using LibraryProjectModule12.Models;
using LibraryProjectModule12.ViewModels;
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
    public class LibrariesControllerTests
    {
        private ApplicationDbContext _context;
        private LibrariesController _controller;

        private const string AdminUserId = "admin-id";
        private const string AdminUserName = "admin@example.com";
        private const string RegularUserId = "user-id";
        private const string RegularUserName = "user@example.com";

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            _controller = new LibrariesController(_context);
            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                new TempDataProvider());

            SeedData();

            // Default to regular authenticated user
            SetUserPrincipal(RegularUserId, RegularUserName, isAuthenticated: true, isAdmin: false);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
            _controller.Dispose();
        }

        private void SeedData()
        {
            // Users
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
            _context.Users.Add(new ApplicationUser
            {
                Id = RegularUserId,
                UserName = RegularUserName,
                Email = RegularUserName,
                Name = "Regular",
                LastName = "User",
                DateRegistrated = DateTime.UtcNow,
                IsDeleted = false
            });

            // Author/Genre/Book
            var author = new Author { Id = 1, Name = "John", LastName = "Doe", Country = "BG", IsDeleted = false };
            var genre = new Genre { Id = 1, Name = "Fiction", Description = "Desc", IsDeleted = false };
            var book1 = new Book { Id = 1, Name = "Book A", Year = 2020, AuthorId = author.Id, GenreId = genre.Id, Description = "Desc", IsDeleted = false };
            var book2 = new Book { Id = 2, Name = "Book B", Year = 2021, AuthorId = author.Id, GenreId = genre.Id, Description = "Desc", IsDeleted = false };
            _context.Authors.Add(author);
            _context.Genres.Add(genre);
            _context.Books.AddRange(book1, book2);

            // Libraries entries
            _context.Libraries.Add(new Library
            {
                Id = 1,
                UserId = RegularUserId,
                BookId = book1.Id,
                ShelfType = ShelfType.WantToRead,
                IsDeleted = false
            });
            _context.Libraries.Add(new Library
            {
                Id = 2,
                UserId = RegularUserId,
                BookId = book2.Id,
                ShelfType = ShelfType.Read,
                IsDeleted = false
            });
            _context.Libraries.Add(new Library
            {
                Id = 3,
                UserId = AdminUserId,
                BookId = book1.Id,
                ShelfType = ShelfType.CurrentlyReading,
                IsDeleted = false
            });

            // A review on book2 to test ReadReviewed grouping
            _context.Reviews.Add(new Review
            {
                Id = 1,
                LibraryId = 2,
                Rating = 5,
                ReviewText = "Great",
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
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.NameIdentifier, userId)
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

        // Index tests

        [Test]
        public async Task Index_Admin_ReturnsAdminIndexView_WithAllEntries()
        {
            SetUserPrincipal(AdminUserId, AdminUserName, isAuthenticated: true, isAdmin: true);

            var result = await _controller.Index() as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo("AdminIndex"));
            var model = result.Model as System.Collections.Generic.List<Library>;
            Assert.That(model, Is.Not.Null);
            // Admin should see all 3 entries
            Assert.That(model.Count, Is.EqualTo(3));
            // Ordered by UserName then Book Name
            Assert.That(model.First().User!.UserName, Is.EqualTo(AdminUserName));
        }

        [Test]
        public async Task Index_RegularUser_ReturnsGroupedShelves_ForCurrentUserOnly()
        {
            var result = await _controller.Index() as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.Null); // default view
            var vm = result.Model as LibraryShelvesViewModel;
            Assert.That(vm, Is.Not.Null);

            // For regular user we seeded 2 entries: WantToRead(book1), Read(book2 with a review)
            Assert.That(vm.WantToRead.Count, Is.EqualTo(1));
            Assert.That(vm.WantToRead[0].BookId, Is.EqualTo(1));

            Assert.That(vm.CurrentlyReading.Count, Is.EqualTo(0));

            Assert.That(vm.ReadReviewed.Count, Is.EqualTo(1));
            Assert.That(vm.ReadReviewed[0].BookId, Is.EqualTo(2));

            Assert.That(vm.ReadNotReviewed.Count, Is.EqualTo(0));
        }

        // AddFromBook tests

        [Test]
        public async Task AddFromBook_Unauthorized_WhenNoUser()
        {
            // No auth
            SetUserPrincipal(userId: "", userName: "", isAuthenticated: false, isAdmin: false);

            var result = await _controller.AddFromBook(1, ShelfType.WantToRead);

            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task AddFromBook_RedirectsToBooksIndex_WhenBookMissing()
        {
            var result = await _controller.AddFromBook(999, ShelfType.WantToRead) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ControllerName, Is.EqualTo("Books"));
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(_controller.TempData.ContainsKey("Error"), Is.True);
        }

        [Test]
        public async Task AddFromBook_RedirectsToBookDetails_WhenDuplicate()
        {
            // Duplicate for RegularUser with book1 already exists (Library Id 1)
            var result = await _controller.AddFromBook(1, ShelfType.WantToRead) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ControllerName, Is.EqualTo("Books"));
            Assert.That(result.ActionName, Is.EqualTo("Details"));
            Assert.That(result.RouteValues!["id"], Is.EqualTo(1));
            Assert.That(_controller.TempData.ContainsKey("Info"), Is.True);
        }

        [Test]
        public async Task AddFromBook_Success_AddsEntry_AndRedirectsToLibrariesIndex()
        {
            // Add a new entry for book2 is duplicate; add for non-existing book for user: use new bookId 3
            var newBook = new Book
            {
                Id = 3,
                Name = "Book C",
                Year = 2022,
                AuthorId = 1,
                GenreId = 1,
                Description = "Desc",
                IsDeleted = false
            };
            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();

            var beforeCount = await _context.Libraries.CountAsync(l => l.UserId == RegularUserId);
            var result = await _controller.AddFromBook(3, ShelfType.CurrentlyReading) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ControllerName, Is.EqualTo("Libraries"));
            Assert.That(result.ActionName, Is.EqualTo("Index"));

            var afterCount = await _context.Libraries.CountAsync(l => l.UserId == RegularUserId);
            Assert.That(afterCount, Is.EqualTo(beforeCount + 1));
            Assert.That(_controller.TempData.ContainsKey("Success"), Is.True);

            var created = await _context.Libraries.OrderByDescending(l => l.Id).FirstAsync(l => l.UserId == RegularUserId && l.BookId == 3);
            Assert.That(created.ShelfType, Is.EqualTo(ShelfType.CurrentlyReading));
        }

        // ChangeShelf tests

        [Test]
        public async Task ChangeShelf_RedirectsWithError_WhenEntryMissing()
        {
            var result = await _controller.ChangeShelf(999, ShelfType.Read) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(_controller.TempData.ContainsKey("Error"), Is.True);
        }

        [Test]
        public async Task ChangeShelf_UpdatesShelf_AndRedirects()
        {
            var entry = await _context.Libraries.FirstAsync(l => l.Id == 3); // Admin’s entry
            Assert.That(entry.ShelfType, Is.EqualTo(ShelfType.CurrentlyReading));

            var result = await _controller.ChangeShelf(entry.Id, ShelfType.Read) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(_controller.TempData.ContainsKey("Success"), Is.True);

            var updated = await _context.Libraries.FirstAsync(l => l.Id == entry.Id);
            Assert.That(updated.ShelfType, Is.EqualTo(ShelfType.Read));
        }

        // Minimal ITempDataProvider
        private sealed class TempDataProvider : ITempDataProvider
        {
            public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
            public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
        }
    }
}
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
using System.Threading.Tasks;

namespace LibraryProject1.Tests
{
    public class BookControllerTests
    {
        private ApplicationDbContext _context;
        private BooksController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            _controller = new BooksController(_context);
            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                new TempDataSerializer());

            SeedData();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
            _controller.Dispose();
        }

        private void SeedData()
        {
            var author = new Author
            {
                Id = 1,
                Name = "John",
                LastName = "Doe",
                Country = "BG",
                IsDeleted = false
            };
            var genre = new Genre
            {
                Id = 1,
                Name = "Fiction",
                Description = "Desc",
                IsDeleted = false
            };

            _context.Authors.Add(author);
            _context.Genres.Add(genre);

            _context.Books.Add(new Book
            {
                Id = 1,
                Name = "Seed Book",
                Year = 2020,
                AuthorId = author.Id,
                GenreId = genre.Id,
                Description = "Seed Description",
                IsDeleted = false
            });

            _context.SaveChanges();
        }

        [Test]
        public async Task Index_ReturnsViewWithBooks()
        {
            var result = await _controller.Index() as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as System.Collections.Generic.List<Book>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count, Is.EqualTo(1));
            Assert.That(model[0].Name, Is.EqualTo("Seed Book"));
        }

        [Test]
        public async Task IndexDelete_ReturnsViewWithDeletedBooks_AdminOnly()
        {
            // soft delete the seeded book
            var b = await _context.Books.FirstAsync();
            b.IsDeleted = true;
            await _context.SaveChangesAsync();

            var result = await _controller.IndexDelete() as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as System.Collections.Generic.List<Book>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count, Is.EqualTo(1));
            Assert.That(model[0].IsDeleted, Is.True);
        }

        [Test]
        public async Task Details_ReturnsNotFound_WhenBookDoesNotExist()
        {
            var result = await _controller.Details(999);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Details_ReturnsView_WhenBookExists()
        {
            var result = await _controller.Details(1) as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as Book;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task Create_Get_ReturnsViewWithEmptyBookAndSelections()
        {
            var result = await _controller.Create() as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as Book;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Id, Is.EqualTo(0)); // new model
            Assert.That(_controller.ViewData["AuthorId"], Is.Not.Null);
            Assert.That(_controller.ViewData["GenreId"], Is.Not.Null);
        }

        [Test]
        public async Task Create_Post_InvalidForeignKeys_ReturnsViewWithModelErrors()
        {
            var model = new Book
            {
                Name = "Invalid FK Book",
                Year = 2001,
                AuthorId = 999, // invalid
                GenreId = 999,  // invalid
                Description = "Desc"
            };

            var result = await _controller.Create(model) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(_controller.ModelState.IsValid, Is.False);
            Assert.That(result.Model, Is.SameAs(model));
            Assert.That(_controller.ViewData["AuthorId"], Is.Not.Null);
            Assert.That(_controller.ViewData["GenreId"], Is.Not.Null);
            Assert.That(_context.Books.Count(), Is.EqualTo(1)); // no new book added
        }

        [Test]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            var model = new Book
            {
                Name = "New Book",
                Year = 2024,
                AuthorId = 1,
                GenreId = 1,
                Description = "New Desc"
            };

            var result = await _controller.Create(model);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(_context.Books.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task Edit_Get_ReturnsNotFound_WhenBookMissing()
        {
            var result = await _controller.Edit(999);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Edit_Get_ReturnsViewWithBook_AndSelections()
        {
            var result = await _controller.Edit(1) as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as Book;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Id, Is.EqualTo(1));
            Assert.That(_controller.ViewData["AuthorId"], Is.Not.Null);
            Assert.That(_controller.ViewData["GenreId"], Is.Not.Null);
        }

        [Test]
        public async Task Edit_Post_IdMismatch_ReturnsBadRequest()
        {
            var existing = await _context.Books.FirstAsync();
            existing.Name = "Updated Name";

            var result = await _controller.Edit(existing.Id + 1, existing);

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task Edit_Post_InvalidForeignKeys_ReturnsViewWithModelErrors()
        {
            var existing = await _context.Books.FirstAsync();
            var invalidModel = new Book
            {
                Id = existing.Id,
                Name = "Invalid FKs",
                Year = 2022,
                AuthorId = 999,
                GenreId = 999,
                Description = "Desc"
            };

            var result = await _controller.Edit(existing.Id, invalidModel) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(_controller.ModelState.IsValid, Is.False);
            Assert.That(result.Model, Is.SameAs(invalidModel));
            Assert.That(_controller.ViewData["AuthorId"], Is.Not.Null);
            Assert.That(_controller.ViewData["GenreId"], Is.Not.Null);
        }

        [Test]
        public async Task Edit_Post_ValidModel_RedirectsToIndex()
        {
            var existing = await _context.Books.FirstAsync();
            var updateModel = new Book
            {
                Id = existing.Id,
                Name = "Updated Book",
                Year = existing.Year,
                AuthorId = 1,
                GenreId = 1,
                Description = existing.Description,
                IsDeleted = existing.IsDeleted
            };

            var result = await _controller.Edit(existing.Id, updateModel);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));

            var updated = await _context.Books.FirstAsync();
            Assert.That(updated.Name, Is.EqualTo("Updated Book"));
        }

        [Test]
        public async Task Delete_Get_ReturnsNotFound_WhenMissing()
        {
            var result = await _controller.Delete(999);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Delete_Get_ReturnsView_WhenBookExists()
        {
            var result = await _controller.Delete(1) as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as Book;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteConfirmed_SetsIsDeletedTrue_AndRedirects()
        {
            var result = await _controller.DeleteConfirmed(1);

            var book = await _context.Books.IgnoreQueryFilters().FirstAsync(b => b.Id == 1);

            Assert.That(book.IsDeleted, Is.True);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));

            Assert.That(_controller.TempData.ContainsKey("Success"), Is.True);
            Assert.That(_controller.TempData.ContainsKey("UndoBookId"), Is.True);
            Assert.That(_controller.TempData["UndoBookId"], Is.EqualTo(1));
        }

        [Test]
        public async Task Restore_Get_ReturnsNotFound_WhenNotDeletedOrMissing()
        {
            // Case 1: Missing
            var missing = await _controller.Restore(999);
            Assert.That(missing, Is.InstanceOf<NotFoundResult>());

            // Case 2: Not deleted
            var notDeleted = await _controller.Restore(1);
            Assert.That(notDeleted, Is.InstanceOf<NotFoundResult>());

            // Mark deleted and verify GET returns view
            var book = await _context.Books.FirstAsync();
            book.IsDeleted = true;
            await _context.SaveChangesAsync();

            var viewResult = await _controller.Restore(1) as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            var model = viewResult.Model as Book;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.IsDeleted, Is.True);
        }

        [Test]
        public async Task Restore_Post_SetsIsDeletedFalse_AndRedirects()
        {
            var book = await _context.Books.FirstAsync();
            book.IsDeleted = true;
            await _context.SaveChangesAsync();

            var model = new Book { Id = 1 };

            var result = await _controller.Restore(model);

            var updated = await _context.Books.IgnoreQueryFilters().FirstAsync(b => b.Id == 1);
            Assert.That(updated.IsDeleted, Is.False);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }

        // Minimal ITempDataProvider serializer for TempDataDictionary creation in tests
        private sealed class TempDataSerializer : ITempDataProvider
        {
            public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
            public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
        }
    }
}

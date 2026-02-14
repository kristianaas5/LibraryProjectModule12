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

namespace LIbraryProjectModule12NUnitTests
{
    public class GenresControllerTests
    {
        private ApplicationDbContext _context;
        private GenresController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            _controller = new GenresController(_context);
            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                new TempDataProvider());

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
            var genre1 = new Genre
            {
                Id = 1,
                Name = "Fiction",
                Description = "Fictional books",
                IsDeleted = false
            };
            var genre2 = new Genre
            {
                Id = 2,
                Name = "History",
                Description = "Historical books",
                IsDeleted = true
            };

            var author = new Author { Id = 1, Name = "John", LastName = "Doe", Country = "BG", IsDeleted = false };
            var book = new Book
            {
                Id = 1,
                Name = "Seed Book",
                Year = 2020,
                AuthorId = author.Id,
                GenreId = genre1.Id,
                Description = "Desc",
                IsDeleted = false
            };

            _context.Authors.Add(author);
            _context.Genres.AddRange(genre1, genre2);
            _context.Books.Add(book);
            _context.SaveChanges();
        }

        [Test]
        public async Task Index_ReturnsViewWithGenres()
        {
            var result = await _controller.Index() as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as System.Collections.Generic.List<Genre>;
            Assert.That(model, Is.Not.Null);
            // Only non-deleted genres should be listed by default query filters
            Assert.That(model.Count, Is.EqualTo(1));
            Assert.That(model[0].Name, Is.EqualTo("Fiction"));
        }

        [Test]
        public async Task IndexDelete_ReturnsViewWithDeletedGenres_AdminOnly()
        {
            var result = await _controller.IndexDelete() as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as System.Collections.Generic.List<Genre>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count, Is.EqualTo(1));
            Assert.That(model[0].IsDeleted, Is.True);
            Assert.That(model[0].Name, Is.EqualTo("History"));
        }

        [Test]
        public async Task Details_ReturnsNotFound_WhenGenreDoesNotExist()
        {
            var result = await _controller.Details(999);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Details_ReturnsView_WhenGenreExists()
        {
            var result = await _controller.Details(1) as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as Genre;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Id, Is.EqualTo(1));
            Assert.That(model.Books, Is.Not.Null);
            Assert.That(model.Books.Count, Is.EqualTo(1));
        }

        [Test]
        public void Create_Get_ReturnsViewWithEmptyGenre()
        {
            var result = _controller.Create() as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as Genre;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Id, Is.EqualTo(0));
        }

        [Test]
        public async Task Create_Post_InvalidModel_ReturnsViewWithModelErrors()
        {
            var model = new Genre
            {
                Name = "", // invalid
                Description = ""
            };
            _controller.ModelState.AddModelError(nameof(Genre.Name), "Required");

            var result = await _controller.Create(model) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(_controller.ModelState.IsValid, Is.False);
            Assert.That(result.Model, Is.SameAs(model));
            Assert.That(_context.Genres.IgnoreQueryFilters().Count(), Is.EqualTo(2)); // no new genre added
        }

        [Test]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            var model = new Genre
            {
                Name = "Science",
                Description = "Science books",
                IsDeleted = false
            };

            var result = await _controller.Create(model);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(_context.Genres.Count(), Is.EqualTo(2)); // Fiction + Science (History is filtered out)
        }

        [Test]
        public async Task Edit_Get_ReturnsNotFound_WhenGenreMissing()
        {
            var result = await _controller.Edit(999);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Edit_Get_ReturnsViewWithGenre()
        {
            var result = await _controller.Edit(1) as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as Genre;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task Edit_Post_IdMismatch_ReturnsBadRequest()
        {
            var existing = await _context.Genres.FirstAsync(g => !g.IsDeleted);
            existing.Name = "Changed";

            var result = await _controller.Edit(existing.Id + 100, existing);

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task Edit_Post_InvalidModel_ReturnsViewWithModelErrors()
        {
            var existing = await _context.Genres.FirstAsync(g => !g.IsDeleted);
            var invalidModel = new Genre
            {
                Id = existing.Id,
                Name = "", // invalid
                Description = existing.Description,
                IsDeleted = existing.IsDeleted
            };
            _controller.ModelState.AddModelError(nameof(Genre.Name), "Required");

            var result = await _controller.Edit(existing.Id, invalidModel) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(_controller.ModelState.IsValid, Is.False);
            Assert.That(result.Model, Is.SameAs(invalidModel));
        }

        [Test]
        public async Task Edit_Post_ValidModel_RedirectsToIndex_AndUpdates()
        {
            var existing = await _context.Genres.FirstAsync(g => !g.IsDeleted);
            var updateModel = new Genre
            {
                Id = existing.Id,
                Name = "Fiction Updated",
                Description = "Updated",
                IsDeleted = existing.IsDeleted
            };

            var result = await _controller.Edit(existing.Id, updateModel);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));

            var updated = await _context.Genres.FirstAsync(g => g.Id == existing.Id);
            Assert.That(updated.Name, Is.EqualTo("Fiction Updated"));
            Assert.That(updated.Description, Is.EqualTo("Updated"));
        }

        [Test]
        public async Task Delete_Get_ReturnsNotFound_WhenMissing()
        {
            var result = await _controller.Delete(999);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Delete_Get_ReturnsView_WhenGenreExists()
        {
            var result = await _controller.Delete(1) as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as Genre;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteConfirmed_SetsIsDeletedTrue_AndRedirects()
        {
            var result = await _controller.DeleteConfirmed(1);

            var genre = await _context.Genres.IgnoreQueryFilters().FirstAsync(g => g.Id == 1);

            Assert.That(genre.IsDeleted, Is.True);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));

            Assert.That(_controller.TempData.ContainsKey("Success"), Is.True);
            Assert.That(_controller.TempData.ContainsKey("UndoGenreId"), Is.True);
            Assert.That(_controller.TempData["UndoGenreId"], Is.EqualTo(1));
        }

        [Test]
        public async Task Restore_Get_ReturnsNotFound_WhenNotDeletedOrMissing()
        {
            // missing
            var missing = await _controller.Restore(999);
            Assert.That(missing, Is.InstanceOf<NotFoundResult>());

            // not deleted (Id=1 initially not deleted)
            var notDeletedResult = await _controller.Restore(1);
            Assert.That(notDeletedResult, Is.InstanceOf<NotFoundResult>());

            // mark deleted and verify GET returns view
            var g = await _context.Genres.FirstAsync(ge => ge.Id == 1);
            g.IsDeleted = true;
            await _context.SaveChangesAsync();

            var viewResult = await _controller.Restore(1) as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            var model = viewResult.Model as Genre;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.IsDeleted, Is.True);
        }

        [Test]
        public async Task Restore_Post_SetsIsDeletedFalse_AndRedirects()
        {
            var g = await _context.Genres.IgnoreQueryFilters().FirstAsync(ge => ge.Id == 2); // already deleted
            Assert.That(g.IsDeleted, Is.True);

            var model = new Genre { Id = 2 };

            var result = await _controller.Restore(model);

            var updated = await _context.Genres.IgnoreQueryFilters().FirstAsync(ge => ge.Id == 2);
            Assert.That(updated.IsDeleted, Is.False);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }

        // Minimal ITempDataProvider for TempDataDictionary
        private sealed class TempDataProvider : ITempDataProvider
        {
            public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
            public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
        }
    }
}
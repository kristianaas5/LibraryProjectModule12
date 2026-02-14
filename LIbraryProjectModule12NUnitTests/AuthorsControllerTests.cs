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
    public class AuthorsControllerTests
    {
        private ApplicationDbContext _context;
        private AuthorsController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            _controller = new AuthorsController(_context);
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
            _context.Authors.Add(new Author
            {
                Id = 1,
                Name = "John",
                LastName = "Doe",
                Country = "BG",
                IsDeleted = false
            });

            _context.SaveChanges();
        }

        [Test]
        public async Task Index_ReturnsViewWithAuthors()
        {
            var result = await _controller.Index() as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as System.Collections.Generic.List<Author>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count, Is.EqualTo(1));
            Assert.That(model[0].LastName, Is.EqualTo("Doe"));
        }

        [Test]
        public async Task IndexDelete_ReturnsViewWithDeletedAuthors_AdminOnly()
        {
            var a = await _context.Authors.FirstAsync();
            a.IsDeleted = true;
            await _context.SaveChangesAsync();

            var result = await _controller.IndexDelete(null) as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as System.Collections.Generic.List<Author>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count, Is.EqualTo(1));
            Assert.That(model[0].IsDeleted, Is.True);
        }

        [Test]
        public async Task Details_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            var result = await _controller.Details(999);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Details_ReturnsView_WhenAuthorExists()
        {
            var result = await _controller.Details(1) as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as Author;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Id, Is.EqualTo(1));
        }

        [Test]
        public void Create_Get_ReturnsViewWithEmptyAuthor()
        {
            var result = _controller.Create() as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as Author;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Id, Is.EqualTo(0));
        }

        [Test]
        public async Task Create_Post_InvalidModel_ReturnsViewWithModelErrors()
        {
            var model = new Author
            {
                // Missing required LastName and Name will fail ModelState if validation triggered manually.
                Country = "BG"
            };
            _controller.ModelState.AddModelError("Name", "Required");
            _controller.ModelState.AddModelError("LastName", "Required");

            var result = await _controller.Create(model) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(_controller.ModelState.IsValid, Is.False);
            Assert.That(result.Model, Is.SameAs(model));
            Assert.That(_context.Authors.Count(), Is.EqualTo(1)); // no new author added
        }

        [Test]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            var model = new Author
            {
                Name = "Jane",
                LastName = "Austen",
                Country = "UK",
                IsDeleted = false
            };

            var result = await _controller.Create(model);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(_context.Authors.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task Edit_Get_ReturnsNotFound_WhenAuthorMissing()
        {
            var result = await _controller.Edit(999);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Edit_Get_ReturnsViewWithAuthor()
        {
            var result = await _controller.Edit(1) as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as Author;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task Edit_Post_IdMismatch_ReturnsBadRequest()
        {
            var existing = await _context.Authors.FirstAsync();
            existing.LastName = "Changed";

            var result = await _controller.Edit(existing.Id + 1, existing);

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task Edit_Post_InvalidModel_ReturnsViewWithModelErrors()
        {
            var existing = await _context.Authors.FirstAsync();
            var invalidModel = new Author
            {
                Id = existing.Id,
                Name = "", // invalid
                LastName = "", // invalid
                Country = "BG",
                IsDeleted = existing.IsDeleted
            };
            _controller.ModelState.AddModelError("Name", "Required");
            _controller.ModelState.AddModelError("LastName", "Required");

            var result = await _controller.Edit(existing.Id, invalidModel) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(_controller.ModelState.IsValid, Is.False);
            Assert.That(result.Model, Is.SameAs(invalidModel));
        }

        [Test]
        public async Task Edit_Post_ValidModel_RedirectsToIndex()
        {
            var existing = await _context.Authors.FirstAsync();
            var updateModel = new Author
            {
                Id = existing.Id,
                Name = "John Updated",
                LastName = "Doe Updated",
                Country = existing.Country,
                IsDeleted = existing.IsDeleted
            };

            var result = await _controller.Edit(existing.Id, updateModel);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));

            var updated = await _context.Authors.FirstAsync();
            Assert.That(updated.Name, Is.EqualTo("John Updated"));
            Assert.That(updated.LastName, Is.EqualTo("Doe Updated"));
        }

        [Test]
        public async Task Delete_Get_ReturnsNotFound_WhenMissing()
        {
            var result = await _controller.Delete(999);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Delete_Get_ReturnsView_WhenAuthorExists()
        {
            var result = await _controller.Delete(1) as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as Author;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteConfirmed_SetsIsDeletedTrue_AndRedirects()
        {
            var result = await _controller.DeleteConfirmed(1);

            var author = await _context.Authors.IgnoreQueryFilters().FirstAsync(a => a.Id == 1);

            Assert.That(author.IsDeleted, Is.True);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));

            Assert.That(_controller.TempData.ContainsKey("Success"), Is.True);
            Assert.That(_controller.TempData.ContainsKey("UndoAuthorId"), Is.True);
            Assert.That(_controller.TempData["UndoAuthorId"], Is.EqualTo(1));
        }

        [Test]
        public async Task Restore_Get_ReturnsNotFound_WhenNotDeletedOrMissing()
        {
            var missing = await _controller.Restore(999);
            Assert.That(missing, Is.InstanceOf<NotFoundResult>());

            var notDeleted = await _context.Authors.FirstAsync();
            Assert.That(notDeleted.IsDeleted, Is.False);

            var notDeletedResult = await _controller.Restore(notDeleted.Id);
            Assert.That(notDeletedResult, Is.InstanceOf<NotFoundResult>());

            notDeleted.IsDeleted = true;
            await _context.SaveChangesAsync();

            var viewResult = await _controller.Restore(notDeleted.Id) as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            var model = viewResult.Model as Author;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.IsDeleted, Is.True);
        }

        [Test]
        public async Task Restore_Post_SetsIsDeletedFalse_AndRedirects()
        {
            var author = await _context.Authors.FirstAsync();
            author.IsDeleted = true;
            await _context.SaveChangesAsync();

            var model = new Author { Id = author.Id };

            var result = await _controller.Restore(model);

            var updated = await _context.Authors.IgnoreQueryFilters().FirstAsync(a => a.Id == author.Id);
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
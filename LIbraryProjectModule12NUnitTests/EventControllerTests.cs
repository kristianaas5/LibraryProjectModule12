
using LibraryProjectModule12.Controllers;
using LibraryProjectModule12.Data;
using LibraryProjectModule12.Models;
using LibraryProjectModule12.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace LIbraryProjectModule12NUnitTests
{
    public class EventsControllerTests
    {
        private ApplicationDbContext _context;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private EventsController _controller;

        [SetUp]
        public void Setup()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;

            _context = new ApplicationDbContext(options);

            _controller = new EventsController(_context, _userManagerMock.Object);

            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>());

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
            _context.Events.Add(new Event
            {
                Id = 1,
                Name = "Test Event",
                Description = "Test Description",
                Date = System.DateTime.Now,
                IsDeleted = false
            });

            _context.SaveChanges();
        }

        [Test]
        public async Task All_ReturnsViewWithEvents()
        {
            var result = await _controller.All() as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as List<EventViewModel>;
            Assert.That(model.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Details_ReturnsNotFound_WhenEventDoesNotExist()
        {
            var result = await _controller.Details(999);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Create_Post_ValidModel_RedirectsToAll()
        {
            var model = new CreateEventViewModel
            {
                Name = "New Event",
                Description = "New Description",
                Date = System.DateTime.Now
            };

            var result = await _controller.Create(model);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect.ActionName, Is.EqualTo("All"));
            Assert.That(_context.Events.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task DeleteConfirmed_SetsIsDeletedTrue()
        {
            var result = await _controller.DeleteConfirmed(1);

            var ev = await _context.Events.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == 1);

            Assert.That(ev.IsDeleted, Is.True);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task Restore_SetsIsDeletedFalse()
        {
            var ev = await _context.Events.FirstAsync();
            ev.IsDeleted = true;
            await _context.SaveChangesAsync();

            var model = new Event { Id = 1 };

            var result = await _controller.Restore(model);

            var updated = await _context.Events.IgnoreQueryFilters()
                .FirstAsync(e => e.Id == 1);

            Assert.That(updated.IsDeleted, Is.False);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
        }
    }
}

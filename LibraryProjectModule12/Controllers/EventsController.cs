namespace LibraryProjectModule12.Controllers;

using LibraryProjectModule12.Data;
using LibraryProjectModule12.Models;
using LibraryProjectModule12.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Authorize]  // Всички actions изискват автентикация
public class EventsController : Controller
{
    // ====================
    // DEPENDENCY INJECTION
    // ====================

    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public EventsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ===========================
    // ALL EVENTS (Всички събития)
    // ===========================

    /// <summary>
    /// GET: /Events/All
    /// Показва всички събития в системата
    /// Достъпно за: User и Admin
    /// </summary>
    public async Task<IActionResult> All()
    {

        // LINQ заявка за взимане на всички събития
        var events = await _context.Events
            .OrderBy(e => e.Date)  // Сортиране 
            .Select(e => new EventViewModel
            {
                Id = e.Id,
                Description = e.Description,
                Name = e.Name,
                Date = e.Date,
            })
            .ToListAsync();

        // SQL който се генерира:
        // SELECT Id, Name, Place, Start, End
        // FROM Events
        // ORDER BY Start

        return View(events);
    }

    public async Task<IActionResult> IndexDelete()
    {

        // LINQ заявка за взимане на всички събития
        var events = await _context.Events.IgnoreQueryFilters() // Include deleted events
            .Where(a => a.IsDeleted)
            .OrderBy(e => e.Date)  // Сортиране 
            .Select(e => new EventViewModel
            {
                Id = e.Id,
                Description = e.Description,
                Name = e.Name,
                Date = e.Date,
            })
            .ToListAsync();

        // SQL който се генерира:
        // SELECT Id, Name, Place, Start, End
        // FROM Events
        // ORDER BY Start

        return View(events);
    }

    // =========================
    // MY EVENTS (Моите събития)
    // =========================

    /// <summary>
    /// GET: /Events/My
    /// Показва събития за които текущият потребител има поръчани билети
    /// Достъпно за: User и Admin
    /// </summary>
    public async Task<IActionResult> My()
    {
        // ───────────────────────────────────────────────────────
        // СТЪПКА 1: Взимаме Id на текущия потребител
        // ───────────────────────────────────────────────────────
        var userId = _userManager.GetUserId(User);

        // User - това е ClaimsPrincipal от authentication cookie
        // GetUserId() извлича claim "sub" (subject) = UserId

        // ───────────────────────────────────────────────────────
        // СТЪПКА 2: Взимаме всички събития с поръчки от потребителя
        // ───────────────────────────────────────────────────────
        var myEvents = await _context.EventUsers
            .Where(o => o.UserId == userId)  // Филтрираме по потребител
            .Include(o => o.Event)               // Eager loading на Event
            .GroupBy(o => o.Event)               // Групираме по събитие
            .Select(g => new EventViewModel
            {
                Id = g.Key.Id,
                Name = g.Key.Name,
                Description = g.Key.Description,
                Date = g.Key.Date,
                // Сумираме всички билети за това събитие
            })
            .OrderBy(e => e.Date)
            .ToListAsync();


        return View(myEvents);
    }

    // ===================================
    // CREATE EVENT (Създаване на събитие)
    //====================================
    /// <summary>
    /// GET: /Events/Create
    /// Показва формата за създаване на ново събитие
    /// Достъпно за: Admin САМО
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]  // САМО Admin
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// POST: /Events/Create
    /// Обработва създаването на ново събитие
    /// Достъпно за: Admin САМО
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateEventViewModel model)
    {
        // СТЪПКА 1: Валидация
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // СТЪПКА 2: Mapping ViewModel -> Entity
        var newEvent = new Event
        {
            Name = model.Name,
            Description = model.Description,
            Date = model.Date
        };

        // СТЪПКА 4: Добавяне в базата данни
        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();

        // SQL който се изпълнява:
        // INSERT INTO Events (Id, Name, Place, Start, End, TotalTickets, PricePerTicket)
        // VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6)

        // СТЪПКА 5: Redirect към All Events

        return RedirectToAction(nameof(All));
    }

    // =================================
    // ORDER TICKETS (Поръчка на билети)
    // =================================

    /// <summary>
    /// GET: /Events/Order/5
    /// Показва формата за поръчка на билети
    /// Достъпно за: User и Admin
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> EventUser(int id)
    {
        // ВАЛИДАЦИЯ: Проверка за null id и връщане на 404
        if (id == null)
        {
            return NotFound();  // 404 ако id е null
        }

        // Намиране на събитието
        var eventItem = await _context.Events.FindAsync(id);

        if (eventItem == null)
        {
            return NotFound();  // 404 ако не съществува
        }

        // Създаване на ViewModel за формата
        var model = new EventUserViewModel
        {
            Id = Guid.NewGuid(),
            eventId = eventItem.Id,
            Name = eventItem.Name,
        };

        return View(model);
    }

    /// <summary>
    /// POST: /Events/Order
    /// Обработва поръчката на билети
    /// Достъпно за: User и Admin
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EventUser(EventViewModel model)
    {
        // СТЪПКА 1: Валидация на модела
        //if (!ModelState.IsValid)
        {
            //return View(model); // Връщаме формата с грешки
        }

        // СТЪПКА 2: Намиране на събитието
        var eventItem = await _context.Events.FindAsync(model.Id);

        if (eventItem == null)
        {
            return NotFound();
        }

        // СТЪПКА 3: Взимане на текущия потребител
        var userId = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();  // Redirect към Login
        }

        // СТЪПКА 5: Създаване на нова поръчка
        var order = new EventUser
        {
            Id = Guid.NewGuid(),
            EventId = model.Id,
            UserId = userId,
        };

        // СТЪПКА 6: Записване в базата данни
        _context.EventUsers.Add(order);
        await _context.SaveChangesAsync();

        // SQL-а който се изпълнява:
        // INSERT INTO Orders (Id, EventId, CustomerId, TicketsCount, OrderedOn)
        // VALUES (@p0, @p1, @p2, @p3, @p4)


        // СТЪПКА 7: Redirect към My Events
        return RedirectToAction(nameof(My));
    }

    // GET: /Books/Details/5
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var _event = await _context.Events
            .FirstOrDefaultAsync(b => b.Id == id);

        if (_event == null) return NotFound();

        var model = new EventViewModel
        {
            Id = _event.Id,
            Name = _event.Name,
            Description = _event.Description,
            Date = _event.Date,
        };

        return View(model);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var _event = await _context.Events.FindAsync(id);
        if (_event == null) return NotFound();
        var model = new EventViewModel
        {
            Id = _event.Id,
            Name = _event.Name,
            Description = _event.Description,
            Date = _event.Date,
        };
        return View(model);
    }

    // POST: /Books/Edit/5
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Event model)
    {
        if (id != model.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(All));
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.Events.AnyAsync(b => b.Id == id);
            if (!exists) return NotFound();
            throw;
        }
    }

    // GET: /Books/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var _event = await _context.Events
            .FirstOrDefaultAsync(b => b.Id == id);

        if (_event == null) return NotFound();
        var model = new EventViewModel
        {
            Id = _event.Id,
            Name = _event.Name,
            Description = _event.Description,
            Date = _event.Date,
        };
        return View(model);
    }

    // POST: /Books/Delete/5
    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var _event = await _context.Events.FirstOrDefaultAsync(b => b.Id == id);
        if (_event == null) return NotFound();

        _event.IsDeleted = true;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Event deleted.";
        TempData["UndoEventId"] = id;

        return RedirectToAction(nameof(All));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Restore(int id)
    {
        var _event = await _context.Events.IgnoreQueryFilters() // Include deleted events
.Where(a => a.IsDeleted)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (_event == null) return NotFound();
        var model = new EventViewModel
        {
            Id = _event.Id,
            Name = _event.Name,
            Description = _event.Description,
            Date = _event.Date,
        };
        return View(model);
    }


        // Optional: Restore soft-deleted event
        [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Event model)
    {
        var _event = await _context.Events.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.Id == model.Id);
        if (_event == null) return NotFound();

        _event.IsDeleted = false;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(All), new { model.Id });
    }
}
namespace LibraryProjectModule12.Controllers;

using LibraryProjectModule12.Data;
using LibraryProjectModule12.Models;
using LibraryProjectModule12.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Authorize]
//Controller for managing events in the library system. It includes actions for listing all events, viewing details, creating new events, editing existing events, and deleting events. Access to certain actions is restricted based on user roles (Admin and User).
public class EventsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public EventsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    //List all events (for both Admin and User)
    public async Task<IActionResult> All()
    {
        var events = await _context.Events
            .OrderBy(e => e.Date)  //Sorting by date
            .Select(e => new EventViewModel
            {
                Id = e.Id,
                Description = e.Description,
                Name = e.Name,
                Date = e.Date,
            })
            .ToListAsync();

        return View(events);
    }
    //List all deleted events (for Admin only)
     [Authorize(Roles = "Admin")]
    public async Task<IActionResult> IndexDelete()
    {

        var events = await _context.Events.IgnoreQueryFilters() // Include deleted events
            .Where(a => a.IsDeleted)
            .OrderBy(e => e.Date)  
            .Select(e => new EventViewModel
            {
                Id = e.Id,
                Description = e.Description,
                Name = e.Name,
                Date = e.Date,
            })
            .ToListAsync();

        return View(events);
    }

    //List events that the current user has ordered (for both Admin and User)
    public async Task<IActionResult> My()
    {
        
        var userId = _userManager.GetUserId(User);// Get the current user's ID

        var myEvents = await _context.EventUsers
            .Where(o => o.UserId == userId)  
            .Include(o => o.Event)              
            .GroupBy(o => o.Event)               
            .Select(g => new EventViewModel
            {
                Id = g.Key.Id,
                Name = g.Key.Name,
                Description = g.Key.Description,
                Date = g.Key.Date,
            })
            .OrderBy(e => e.Date)
            .ToListAsync();


        return View(myEvents);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]  // Only Admin can access the form to create a new event
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    // Creates a new event. It first validates the incoming model, and if it's valid, it maps the CreateEventViewModel to an Event entity, adds it to the database context, and saves the changes. After successfully creating the event, it redirects the user to the list of all events.
    public async Task<IActionResult> Create(CreateEventViewModel model)
    {
        // Validation: Check if the model state is valid. If it is not, return the view with the model to display validation errors.
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        // Mapping the ViewModel to the Entity
        var newEvent = new Event
        {
            Name = model.Name,
            Description = model.Description,
            Date = model.Date
        };
        // Adding the new event to the database context and saving changes
        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(All));
    }

    [HttpGet]
    // GET: /Events/Order/5
    // Displays the form for ordering tickets for a specific event. It first checks if the provided event ID is null and returns a 404 Not Found if it is. Then, it attempts to find the event in the database using the provided ID. If the event does not exist, it also returns a 404 Not Found. If the event is found, it creates an EventUserViewModel with the event details and returns the view to display the order form.
    public async Task<IActionResult> EventUser(int id)
    {
        //Validation: Check if the event ID is null. If it is, return a 404 Not Found response.
        if (id == null)
        {
            return NotFound();  // 404 
        }
        // Attempt to find the event in the database using the provided ID. If the event does not exist, return a 404 Not Found response.
        var eventItem = await _context.Events.FindAsync(id);

        if (eventItem == null)
        {
            return NotFound();  // 404
        }
        // If the event is found, create an EventUserViewModel with the event details and return the view to display the order form.
        var model = new EventUserViewModel
        {
            Id = Guid.NewGuid(),
            eventId = eventItem.Id,
            Name = eventItem.Name,
        };

        return View(model);
    }

    // POST: /Events/Order
    [HttpPost]
    [ValidateAntiForgeryToken]
    // Handles the submission of the order form for an event. It performs several steps: validating the model, finding the event, getting the current user's ID, creating a new EventUser entity to represent the order, saving it to the database, and finally redirecting the user to their list of events.
    public async Task<IActionResult> EventUser(EventViewModel model)
    {
        var eventItem = await _context.Events.FindAsync(model.Id);
        //Validation: Check if the event exists. If it does not, return a 404 Not Found response.
        if (eventItem == null)
        {
            return NotFound();
        }
        // Get the current user's ID using the UserManager. If the user is not authenticated, return a Challenge result to prompt for authentication.
        var userId = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();  // Prompt for authentication if the user is not authenticated
        }
        // Create a new EventUser entity to represent the order, associating the current user with the selected event. Add this entity to the database context and save the changes to persist the order in the database.
        var order = new EventUser
        {
            Id = Guid.NewGuid(),
            EventId = model.Id,
            UserId = userId,
        };
        // Add the new order to the database context and save changes

        _context.EventUsers.Add(order);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(My));
    }

    // GET: /Books/Details/5
    [AllowAnonymous]
    // Displays the details of a specific event. It first attempts to find the event in the database using the provided ID. If the event does not exist, it returns a 404 Not Found response. If the event is found, it creates an EventViewModel with the event details and returns the view to display the information.
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
    // GET: /Books/Edit/5
    // Displays the form for editing an existing event. It first attempts to find the event in the database using the provided ID. If the event does not exist, it returns a 404 Not Found response. If the event is found, it creates an EventViewModel with the event details and returns the view to display the edit form.
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
    // Handles the submission of the edit form for an existing event. It first checks if the provided ID matches the ID of the event in the model. If they do not match, it returns a BadRequest response. Then, it validates the model state, and if it is not valid, it returns the view with the model to display validation errors. If the model is valid, it updates the event in the database context and saves the changes. If a concurrency exception occurs during saving, it checks if the event still exists in the database; if it does not exist, it returns a 404 Not Found response. If it does exist, it rethrows the exception.
    public async Task<IActionResult> Edit(int id, Event model)
    {
        if (id != model.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            return View(model);// Return the view with the model to display validation errors if the model state is not valid.
        }
        // Update the event in the database context and save changes. If a concurrency exception occurs, check if the event still exists; if it does not, return a 404 Not Found response. If it does exist, rethrow the exception.
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
    // Displays the confirmation page for deleting an event. It first attempts to find the event in the database using the provided ID. If the event does not exist, it returns a 404 Not Found response. If the event is found, it creates an EventViewModel with the event details and returns the view to display the delete confirmation.
    public async Task<IActionResult> Delete(int id)
    {
        // Attempt to find the event in the database using the provided ID. If the event does not exist, return a 404 Not Found response.
        var _event = await _context.Events
            .FirstOrDefaultAsync(b => b.Id == id);
        //Validation: Check if the event exists. If it does not, return a 404 Not Found response.
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
    // Handles the confirmation of deleting an event. It first attempts to find the event in the database using the provided ID. If the event does not exist, it returns a 404 Not Found response. If the event is found, it marks the event as deleted by setting its IsDeleted property to true and saves the changes to the database. After successfully marking the event as deleted, it sets a success message in TempData and redirects the user to the list of all events.
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
    // GET: /Books/Restore/5
    // Displays the confirmation page for restoring a soft-deleted event. It first attempts to find the event in the database, including those that are marked as deleted, using the provided ID. If the event does not exist or is not marked as deleted, it returns a 404 Not Found response. If the event is found and is marked as deleted, it creates an EventViewModel with the event details and returns the view to display the restore confirmation.
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
    // Handles the confirmation of restoring a soft-deleted event. It first attempts to find the event in the database, including those that are marked as deleted, using the provided ID. If the event does not exist, it returns a 404 Not Found response. If the event is found, it marks the event as not deleted by setting its IsDeleted property to false and saves the changes to the database. After successfully restoring the event, it redirects the user to the list of all events.
    public async Task<IActionResult> Restore(Event model)
    {
        var _event = await _context.Events.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.Id == model.Id);
        if (_event == null) return NotFound();

        _event.IsDeleted = false;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(All), new { model.Id });
    }
}
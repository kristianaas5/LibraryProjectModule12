namespace LibraryProjectModule12.ViewModels;
using System.ComponentModel.DataAnnotations;
/// <summary>
/// EventUserViewModel is a view model class that represents the association between an event and a user in the context of a library project. It contains properties for the unique identifier of the event, the foreign key for the associated user, and the name of the event. This view model is used to transfer data between the application and the user interface when displaying information about events and their associated users.
/// </summary>
public class EventUserViewModel
{
    // The unique identifier for the event. This property is required and serves as the primary key for the event entity.
    public Guid Id { get; set; }
    //  The foreign key for the user associated with the event. This property is required and references the ApplicationUser entity.
    public int eventId { get; set; }
    // The foreign key for the user associated with the event. This property is required and references the ApplicationUser entity.

    public string Name { get; set; } = string.Empty;

}
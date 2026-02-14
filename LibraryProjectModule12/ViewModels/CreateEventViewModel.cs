using System.ComponentModel.DataAnnotations;

namespace LibraryProjectModule12.ViewModels
{
    /// <summary>
    /// CreateEventViewModel is a view model class used for creating new events. It contains properties for the event's name, description, and date, along with validation attributes to ensure that the required fields are filled out correctly.
    /// </summary>
    public class CreateEventViewModel
    {
        // The name of the event. This property is required, has a maximum length of 256 characters, and is displayed with the name "Event Name".
        [Required(ErrorMessage = "Името е задължително")]
        [MaxLength(256)]
        [Display(Name = "Event Name")]
        public string Name { get; set; } = string.Empty;
        // The description of the event. This property is required, has a maximum length of 2048 characters, and is displayed with the name "Event Description".
        [Required(ErrorMessage = "Описанието е задължително")]
        [MaxLength(2048)]
        [Display(Name = "Event Description")]
        public string? Description { get; set; }
        //  The date and time of the event. This property is required and is displayed with the name "Event Date and Time".
        [Required(ErrorMessage = "Датата е задължителна")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Event Date and Time")]
        public DateTime Date { get; set; }
    }
}

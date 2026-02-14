using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LibraryProjectModule12.Models
{
    /// <summary>
    /// ApplicationUser class represents a user in the application and extends the IdentityUser class provided by
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        // The first name of the user. This property is required, has a maximum length of 128 characters, and is displayed with the name "First Name".
        [Required]
        [MaxLength(128)]
        [Display(Name = "First Name")]
        public string Name { get; set; } = string.Empty;
        // The last name of the user. This property is required, has a maximum length of 128 characters, and is displayed with the name "Last Name".
        [Required]
        [MaxLength(128)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        // The date when the user registered. This property is required, is of type DateTime, and is displayed with the name "Date of Registration".
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Registration")]
        public DateTime DateRegistrated { get; set; }
        // A boolean property indicating whether the user has been marked as deleted. This property is required and allows us to track the deletion status of the user without physically removing it from the database.
        [Required]
        public bool IsDeleted { get; set; }
        // A collection of EventUser entities representing the events that the user is associated with. This property is initialized to an empty list to avoid null reference issues and allows us to navigate from a user to the events they are participating in.
        public ICollection<EventUser> EventUsers { get; set; } = new List<EventUser>();
        // A collection of Library entities representing the user's library entries. This property is initialized to an empty list to avoid null reference issues and allows us to navigate from a user to their library entries, which represent the books they have in their library and how they are categorized on different shelves.
        public ICollection<Library> Libraries { get; set; } = new List<Library>();
    }
}
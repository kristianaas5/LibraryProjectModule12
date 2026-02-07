using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LibraryProjectModule12.Models
{
    public class User : IdentityUser
    {

        [Required]
        [MaxLength(128)]
        [Display(Name = "First Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(128)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Registration")]
        public DateTime DateRegistrated { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public ICollection<EventUser> EventUsers { get; set; } = new List<EventUser>();
        public ICollection<Library> Libraries { get; set; } = new List<Library>();
    }
}
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LibraryProjectModule12.Models
{
    public class User : IdentityUser
    {
        public int Id { get; set; }

        [Required, MaxLength(128)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(128)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(256), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(32), Phone]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateRegistrated { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<EventUser> EventUsers { get; set; } = new List<EventUser>();
        public ICollection<Library> Libraries { get; set; } = new List<Library>();
    }
}
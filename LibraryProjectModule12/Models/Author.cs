using System.ComponentModel.DataAnnotations;

namespace LibraryProjectModule12.Models
{
    /// <summary>
    /// Class representing an author in the library system. Contains properties for the author's ID, name, last name, country of origin, deletion status, and a collection of books associated with the author.
    /// </summary>
    public class Author
    {
        // The unique identifier for the author. This property is required.
        [Required]
        public int Id { get; set; }

        // The first name of the author. This property is required and has a maximum length of 128 characters.
        [Required]
        [MaxLength(128)]
        [Display(Name = "First Name")]
        public string Name { get; set; } = string.Empty;

        // The last name of the author. This property is required and has a maximum length of 128 characters.
        [Required]
        [MaxLength(128)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        // The country of origin of the author. This property is optional and has a maximum length of 128 characters.
        [MaxLength(128)]
        [Display(Name = "Country of Origin")]
        public string? Country { get; set; }

        // Indicates whether the author has been marked as deleted. This property is required.
        [Required]
        public bool IsDeleted { get; set; }

        // A collection of books associated with the author. This property is initialized to an empty list to avoid null reference issues.
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
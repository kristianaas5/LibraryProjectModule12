using System.ComponentModel.DataAnnotations;

namespace LibraryProjectModule12.Models
{
    public class Author
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        [Display(Name = "First Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(128)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(128)]
        [Display(Name = "Country of Origin")]
        public string? Country { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
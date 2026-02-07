using System.ComponentModel.DataAnnotations;

namespace LibraryProjectModule12.Models
{
    public class Genre
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        [Display(Name = "Genre Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(1024)]
        [Display(Name = "Genre Description")]
        public string? Description { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
using System.ComponentModel.DataAnnotations;

namespace LibraryProjectModule12.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required, MaxLength(128)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1024)]
        public string? Description { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
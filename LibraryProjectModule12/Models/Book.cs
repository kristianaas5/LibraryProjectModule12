using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProjectModule12.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required, MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        public int Year { get; set; }

        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }

        public Author? Author { get; set; }

        [ForeignKey(nameof(Genre))]
        public int GenreId { get; set; }

        public Genre? Genre { get; set; }

        [MaxLength(2048)]
        public string? Description { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Library> Libraries { get; set; } = new List<Library>();
    }
}
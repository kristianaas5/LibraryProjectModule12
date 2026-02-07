using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProjectModule12.Models
{
    public class Book
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        [Display(Name = "Book Title")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Year must be a non-negative integer.")]
        [Display(Name = "Publication Year")]
        public int Year { get; set; }

        [Required]
        [ForeignKey(nameof(Author))]
        [Display(Name = "Author")]
        public int AuthorId { get; set; }

        public Author? Author { get; set; }

        [Required]
        [ForeignKey(nameof(Genre))]
        [Display(Name = "Genre")]
        public int GenreId { get; set; }

        public Genre? Genre { get; set; }

        [Required]
        [MaxLength(2048)]
        [Display(Name = "Book Description")]
        public string? Description { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public ICollection<Library> Libraries { get; set; } = new List<Library>();
    }
}
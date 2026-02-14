using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProjectModule12.Models
{
    /// <summary>
    /// Book class represents a book entity in the library system. It contains properties such as Id, Name, Year, AuthorId, GenreId, Description, IsDeleted, and navigation properties for Author, Genre, and Libraries.
    /// </summary>
    public class Book
    {
        // The unique identifier for the book. This property is required.
        [Required]
        public int Id { get; set; }
        // The title of the book. This property is required and has a maximum length of 256 characters.
        [Required]
        [MaxLength(256)]
        [Display(Name = "Book Title")]
        public string Name { get; set; } = string.Empty;
        // The publication year of the book. This property is required and must be a non-negative integer.
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Year must be a non-negative integer.")]
        [Display(Name = "Publication Year")]
        public int Year { get; set; }
        // The foreign key for the author of the book. This property is required and references the Author entity.
        [Required]
        [ForeignKey(nameof(Author))]
        [Display(Name = "Author")]
        public int AuthorId { get; set; }
        // Navigation property for the author of the book. This property is optional and allows access to the related Author entity.
        public Author? Author { get; set; }
        // The foreign key for the genre of the book. This property is required and references the Genre entity.
        [Required]
        [ForeignKey(nameof(Genre))]
        [Display(Name = "Genre")]
        public int GenreId { get; set; }
        //  Navigation property for the genre of the book. This property is optional and allows access to the related Genre entity.
        public Genre? Genre { get; set; }
        // A brief description of the book. This property is required and has a maximum length of 2048 characters.
        [Required]
        [MaxLength(2048)]
        [Display(Name = "Book Description")]
        public string? Description { get; set; }
        // Indicates whether the book has been marked as deleted. This property is required.
        [Required]
        public bool IsDeleted { get; set; }
        // A collection of libraries that have this book in their collection. This property is initialized to an empty list to avoid null reference issues.
        public ICollection<Library> Libraries { get; set; } = new List<Library>();
    }
}
using System.ComponentModel.DataAnnotations;

namespace LibraryProjectModule12.Models
{
    /// <summary>
    /// Genre represents a category or type of books in the library system. It contains properties for the genre's name, description, and a flag to indicate if it has been deleted. Each genre can be associated with multiple books, allowing for categorization and organization of the library's collection.
    /// </summary>
    public class Genre
    {
        // The unique identifier for the genre. This property is required and serves as the primary key for the Genre entity.
        [Required]
        public int Id { get; set; }
        // The name of the genre. This property is required, has a maximum length of 128 characters, and is displayed with the name "Genre Name".
        [Required]
        [MaxLength(128)]
        [Display(Name = "Genre Name")]
        public string Name { get; set; } = string.Empty;
        // A description of the genre. This property is required, has a maximum length of 1024 characters, and is displayed with the name "Genre Description".
        [Required]
        [MaxLength(1024)]
        [Display(Name = "Genre Description")]
        public string? Description { get; set; }
        // A boolean property indicating whether the genre has been marked as deleted. This property is required and allows us to track the deletion status of the genre without physically removing it from the database.
        [Required]
        public bool IsDeleted { get; set; }
        // A collection of books associated with this genre. This property is initialized to an empty list to avoid null reference issues and allows us to navigate from a genre to the books that belong to it.
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
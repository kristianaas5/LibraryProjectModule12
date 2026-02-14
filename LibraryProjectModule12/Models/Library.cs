using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProjectModule12.Models
{
    /// <summary>
    /// Library class represents a user's collection of books in the library system. It contains properties such as Id, UserId, BookId, ShelfType, IsDeleted, and navigation properties for User, Book, and Reviews. This class allows us to track which books are in a user's library and how they are categorized on different shelves.
    /// <sumary/>
    public class Library
    {
        // The unique identifier for the library entry. This property is required and serves as the primary key for the Library entity.
        [Required]
        public int Id { get; set; }
        // The foreign key for the user who owns this library entry. This property is required and references the ApplicationUser entity.
        [Required]
        [ForeignKey(nameof(User))]
        [Display(Name = "User")]
        public string UserId { get; set; }
        // Navigation property for the user who owns this library entry. This property is optional and allows access to the related ApplicationUser entity.
        public ApplicationUser? User { get; set; }

        [Required]// The foreign key for the book associated with this library entry. This property is required and references the Book entity.
        [ForeignKey(nameof(Book))]
        [Display(Name = "Book")]
        public int BookId { get; set; }
        // Navigation property for the book associated with this library entry. This property is optional and allows access to the related Book entity.
        public Book? Book { get; set; }
        
        // The type of shelf where the book is categorized in the user's library. This property is required and is displayed with the name "Shelf Type". It uses the ShelfType enumeration to define the possible values for categorizing books on different shelves.
        [Required]
        [Display(Name = "Shelf Type")]
        public ShelfType ShelfType { get; set; }
        // A boolean property indicating whether the library entry has been marked as deleted. This property is required and allows us to track the deletion status of the library entry without physically removing it from the database.
        [Required]
        public bool IsDeleted { get; set; }
        // A collection of reviews associated with this library entry. This property is initialized to an empty list to avoid null reference issues and allows us to navigate from a library entry to the reviews that belong to it.
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
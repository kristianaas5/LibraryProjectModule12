using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProjectModule12.Models
{
    /// <summary>
    /// Review class represents a review entity in the library system. It contains properties such as Id, LibraryId, Rating, ReviewText, IsDeleted, and navigation property for Library. This class allows users to provide feedback and ratings for the books in their library collection.
    /// </summary>
    public class Review
    {
        // The unique identifier for the review. This property is required and serves as the primary key for the Review entity.
        [Required]
        public int Id { get; set; }
        // The foreign key for the library entry associated with this review. This property is required and references the Library entity.
        [Required]
        [ForeignKey(nameof(Library))]
        [Display(Name = "Library")]
        public int LibraryId { get; set; }
        // Navigation property for the library entry associated with this review. This property is optional and allows access to the related Library entity.
        public Library? Library { get; set; }
        // The rating given in the review. This property is required, must be an integer between 1 and 5, and is displayed with the name "Rating". It allows users to provide a numerical rating for the book they are reviewing.
        [Required]
        [Range(1, 5)]
        [Display(Name = "Rating")]
        public int Rating { get; set; }
        // The text of the review. This property is required, has a maximum length of 2048 characters, and is displayed with the name "Review Text". It allows users to provide detailed feedback and comments about the book they are reviewing.
        [MaxLength(2048)]
        [Display(Name = "Review Text")]
        public string? ReviewText { get; set; }
        // A boolean property indicating whether the review has been marked as deleted. This property is required and allows us to track the deletion status of the review without physically removing it from the database.
        public bool IsDeleted { get; set; }
    }
}
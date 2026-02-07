using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProjectModule12.Models
{
    public class Review
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Library))]
        [Display(Name = "Library")]
        public int LibraryId { get; set; }

        public Library? Library { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Rating")]
        public int Rating { get; set; }

        [MaxLength(2048)]
        [Display(Name = "Review Text")]
        public string? ReviewText { get; set; }

        public bool IsDeleted { get; set; }
    }
}
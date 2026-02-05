using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProjectModule12.Models
{
    public class Review
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Library))]
        public int LibraryId { get; set; }

        public Library? Library { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(2048)]
        public string? ReviewText { get; set; }

        public bool IsDeleted { get; set; }
    }
}
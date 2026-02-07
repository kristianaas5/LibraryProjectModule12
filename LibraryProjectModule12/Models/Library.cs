using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProjectModule12.Models
{
    public class Library
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public User? User { get; set; }

        [Required]
        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }

        public Book? Book { get; set; }

        [Required]
        public ShelfType ShelfType { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
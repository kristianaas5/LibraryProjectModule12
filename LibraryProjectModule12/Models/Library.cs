using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProjectModule12.Models
{
    public class Library
    {
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public User? User { get; set; }

        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }

        public Book? Book { get; set; }

        [Required]
        public ShelfType ShelfType { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
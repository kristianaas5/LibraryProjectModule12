using System.ComponentModel.DataAnnotations;

namespace LibraryProjectModule12.Models
{
    public class Event
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(2048)]
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public ICollection<EventUser> EventUsers { get; set; } = new List<EventUser>();
    }
}
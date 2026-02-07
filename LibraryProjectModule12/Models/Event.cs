using System.ComponentModel.DataAnnotations;

namespace LibraryProjectModule12.Models
{
    public class Event
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        [Display(Name = "Event Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(2048)]
        [Display(Name = "Event Description")]
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Event Date and Time")]
        public DateTime Date { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public ICollection<EventUser> EventUsers { get; set; } = new List<EventUser>();
    }
}
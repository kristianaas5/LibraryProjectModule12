using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProjectModule12.Models
{
    public class EventUser
    {
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public User? User { get; set; }

        [ForeignKey(nameof(Event))]
        public int EventId { get; set; }

        public Event? Event { get; set; }

        public bool IsDeleted { get; set; }
    }
}
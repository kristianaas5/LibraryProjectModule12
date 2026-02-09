using System.ComponentModel.DataAnnotations;

namespace LibraryProjectModule12.ViewModels
{
    public class EventViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Името е задължително")]
        [MaxLength(256)]
        [Display(Name = "Event Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Описанието е задължително")]
        [MaxLength(2048)]
        [Display(Name = "Event Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Датата е задължителна")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Event Date and Time")]
        public DateTime Date { get; set; }
    }
}

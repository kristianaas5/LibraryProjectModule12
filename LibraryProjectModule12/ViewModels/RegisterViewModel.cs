using System.ComponentModel.DataAnnotations;
using System.Data;

namespace LibraryProjectModule12.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Потребителското име е задължително")]
        [StringLength(50, MinimumLength = 3,
        ErrorMessage = "Потребителското име трябва да е между 3 и 50 символа")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Името е задължително")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Името трябва да е между 2 и 50 символа")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Фамилията е задължителна")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Фамилията трябва да е между 2 и 50 символа")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email адресът е задължителен")]
        [EmailAddress(ErrorMessage = "Невалиден email формат")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Паролата е задължителна")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "Паролата трябва да е поне 3 символа")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Потвърждението на паролата е задължително")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролите не съвпадат")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

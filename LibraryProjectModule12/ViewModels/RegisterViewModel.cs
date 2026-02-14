using System.ComponentModel.DataAnnotations;
using System.Data;

namespace LibraryProjectModule12.ViewModels
{
    /// <summary>
    /// Represents the data required for user registration, including username, first name, last name, email, password, and password confirmation. Each property is decorated with validation attributes to ensure that the input meets specific criteria, such as required fields, string length limits, email format, and password confirmation matching.
    /// </summary>
    public class RegisterViewModel
    {
        // The username of the user. This property is required, must be between 3 and 50 characters long, and is displayed with the name "Username".
        [Required(ErrorMessage = "Потребителското име е задължително")]
        [StringLength(50, MinimumLength = 3,
        ErrorMessage = "Потребителското име трябва да е между 3 и 50 символа")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;
        // The first name of the user. This property is required, must be between 2 and 50 characters long, and is displayed with the name "First Name".
        [Required(ErrorMessage = "Името е задължително")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Името трябва да е между 2 и 50 символа")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
        // The last name of the user. This property is required, must be between 2 and 50 characters long, and is displayed with the name "Last Name".
        [Required(ErrorMessage = "Фамилията е задължителна")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Фамилията трябва да е между 2 и 50 символа")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        // The email address of the user. This property is required, must be in a valid email format, and is displayed with the name "Email".
        [Required(ErrorMessage = "Email адресът е задължителен")]
        [EmailAddress(ErrorMessage = "Невалиден email формат")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        // The password of the user. This property is required, must be between 3 and 100 characters long, is of type password, and is displayed with the name "Password".

        [Required(ErrorMessage = "Паролата е задължителна")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "Паролата трябва да е поне 3 символа")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
        // The confirmation of the password. This property is required, must match the "Password" property, is of type password, and is displayed with the name "Confirm Password".

        [Required(ErrorMessage = "Потвърждението на паролата е задължително")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролите не съвпадат")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

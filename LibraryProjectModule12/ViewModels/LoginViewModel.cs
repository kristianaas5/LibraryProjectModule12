using System.ComponentModel.DataAnnotations;

namespace LibraryProjectModule12.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Потребителското име е задължително")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Паролата е задължителна")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}

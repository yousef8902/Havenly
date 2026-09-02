using System.ComponentModel.DataAnnotations;

namespace Havenly.BLL.ModelVMs.Account
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;
    }
}

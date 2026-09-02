using System.ComponentModel.DataAnnotations;

namespace Havenly.BLL.ModelVMs.Account
{
    public class VerifyResetOtpVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Verification code is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be exactly 6 digits")]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "Code must consist of 6 numbers")]
        public string OtpCode { get; set; } = string.Empty;
    }
}

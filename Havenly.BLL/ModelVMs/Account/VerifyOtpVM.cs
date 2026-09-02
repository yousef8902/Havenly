using System.ComponentModel.DataAnnotations;

namespace Havenly.BLL.ModelVMs.Account
{
    public class VerifyOtpVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the 6-digit verification code.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Verification code must be exactly 6 digits.")]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "Verification code must contain digits only.")]
        public string OtpCode { get; set; } = string.Empty;
    }
}

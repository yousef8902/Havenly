using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Havenly.BLL.ModelVMs.Account
{
    public class RegisterVM
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Guest";

        public IFormFile? VerificationDocument { get; set; }
        public string? VerificationDocumentUrl { get; set; }
    }
}
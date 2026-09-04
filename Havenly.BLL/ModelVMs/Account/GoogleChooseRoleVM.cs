using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Havenly.BLL.ModelVMs.Account
{
    public class GoogleChooseRoleVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? PictureUrl { get; set; }

        public string ReturnUrl { get; set; } = "/";

        [Required]
        public string Role { get; set; } = "Guest";

        public IFormFile? VerificationDocument { get; set; }
    }
}

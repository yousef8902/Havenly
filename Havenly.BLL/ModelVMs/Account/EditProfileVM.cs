using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Havenly.BLL.ModelVMs.Account
{
    public class EditProfileVM
    {
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        [StringLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters")]
        public string? Bio { get; set; }

        public string? CurrentProfilePictureUrl { get; set; }

        public IFormFile? ProfilePictureFile { get; set; }
    }
}

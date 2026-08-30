using System.ComponentModel.DataAnnotations;

<<<<<<< HEAD
namespace Havenly.BLL.ModelVMs.Account
=======
namespace Havenly.BLL.ModelVM.Account
>>>>>>> 63b1b37 (add search by review and change relation between (review->booking) to (review->user))
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
    }
}
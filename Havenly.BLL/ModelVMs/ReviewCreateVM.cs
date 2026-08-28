using System.ComponentModel.DataAnnotations;

namespace Havenly.BLL.ModelVMs
{

    public class ReviewCreateVM
    {
        [Required(ErrorMessage = "Choose a stay to review.")]
        [Display(Name = "Stay")]
        public string BookingId { get; set; } = "";

        [Range(1, 5)]
        public int Rating { get; set; } = 5;

        [Required]
        [MinLength(20, ErrorMessage = "Please write at least 20 characters.")]
        [Display(Name = "Your review")]
        public string Body { get; set; } = "";
    }
}
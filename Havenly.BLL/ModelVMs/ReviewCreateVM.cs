using System;
using System.ComponentModel.DataAnnotations;

namespace Havenly.BLL.ModelVMs
{
    public class ReviewCreateVM
    {
        [Required(ErrorMessage = "Stay ID is required.")]
        public long BookingId { get; set; }

        public long PropertyId { get; set; }
        public string PropertyName { get; set; } = "";
        public string PropertyImage { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
        public string HostName { get; set; } = "";
        public string HostAvatar { get; set; } = "";
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        [Range(1, 5, ErrorMessage = "Please select a rating between 1 and 5 stars.")]
        public int Rating { get; set; } = 5;

        [Required(ErrorMessage = "Please write a comment sharing your experience.")]
        [MinLength(10, ErrorMessage = "Please write at least 10 characters.")]
        [MaxLength(1000, ErrorMessage = "Review cannot exceed 1000 characters.")]
        [Display(Name = "Your review")]
        public string Comment { get; set; } = "";
    }

    public class ReviewRespondVM
    {
        public long ReviewId { get; set; }
        public long PropertyId { get; set; }
        public string PropertyName { get; set; } = "";
        public string GuestName { get; set; } = "";
        public int Rating { get; set; }
        public string GuestComment { get; set; } = "";

        [Required(ErrorMessage = "Please enter your response.")]
        [MinLength(5, ErrorMessage = "Response must be at least 5 characters.")]
        [MaxLength(1000, ErrorMessage = "Response cannot exceed 1000 characters.")]
        [Display(Name = "Host Response")]
        public string Response { get; set; } = "";
    }

    public class ReviewSubmitResultVM
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public long ReviewId { get; set; }
        public long PropertyId { get; set; }
        public string PropertyName { get; set; } = "";
        public string HostEmail { get; set; } = "";
        public string HostName { get; set; } = "";
        public string GuestName { get; set; } = "";
        public int Rating { get; set; }
        public string Comment { get; set; } = "";
    }
}
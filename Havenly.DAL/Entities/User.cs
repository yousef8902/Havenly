using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havenly.DAL.Entities;

public class User
{
    [Key]
    public long UserID { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; }

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(50)]
    public string Role { get; set; }

    public bool IsDeleted { get; set; }

    // Navigation Properties
    [InverseProperty("Owner")]
    public ICollection<Property> Properties { get; set; }

    [InverseProperty("Guest")]
    public ICollection<Booking> Bookings { get; set; }
        
    public ICollection<Favorite> Favorites { get; set; }
    public ICollection<Review> Reviews { get; set; }
}

using Havenly.DAL.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havenly.DAL.Entities;

public class User : IdentityUser
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    public UserStatus Status { get; set; }

    // Navigation Properties

    [InverseProperty("Owner")]
    public ICollection<Property> Properties { get; set; }

    [InverseProperty("Guest")]
    public ICollection<Booking> Bookings { get; set; }

    public ICollection<Favorite> Favorites { get; set; }

    public ICollection<Review> Reviews { get; set; }


    // Create User
    public void Create(string name, string email)
    {
        Name = name;

        // Identity properties
        Email = email;
        UserName = email;

        Status = UserStatus.Active;

        // Initialize navigation properties
        Properties = new List<Property>();
        Bookings = new List<Booking>();
        Favorites = new List<Favorite>();
        Reviews = new List<Review>();
    }


    // Update User
    public void Update(string name, string email)
    {
        Name = name;

        Email = email;
        UserName = email;
    }


    // Delete User
    public void Delete()
    {
        Status = UserStatus.Deleted;
    }
}

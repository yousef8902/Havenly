
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

    [StringLength(50)]
    public string Role { get; set; } = UserRoles.Guest;

    public UserStatus Status { get; set; }

    [StringLength(300)]
    public string? ProfilePictureUrl { get; set; }

    [StringLength(1000)]
    public string? Bio { get; set; }

    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

    // Navigation Properties

    [InverseProperty("Owner")]
    public ICollection<Property> Properties { get; set; }

    [InverseProperty("Guest")]
    public ICollection<Booking> Bookings { get; set; }

    public ICollection<Favorite> Favorites { get; set; }

    public ICollection<Review> Reviews { get; set; }


    // Create User
    public void Create(string name, string email, string role = UserRoles.Guest)
    {
        Name = name;
        Email = email;
        UserName = email;
        Role = role;
        Status = UserStatus.Active;

        // Initialize navigation properties
        Properties = new List<Property>();
        Bookings = new List<Booking>();
        Favorites = new List<Favorite>();
        Reviews = new List<Review>();
    }

    public void Create(string name, string passwordHash, string email, string role)
    {
        Name = name;
        PasswordHash = passwordHash;
        Email = email;
        UserName = email;
        Role = role;
        Status = UserStatus.Active;

        Properties = new List<Property>();
        Bookings = new List<Booking>();
        Favorites = new List<Favorite>();
        Reviews = new List<Review>();
    }


    // Update User
    public void Update(string name, string email, string role = UserRoles.Guest)
    {
        Name = name;
        Email = email;
        UserName = email;
        Role = role;
    }

    public void Update(string name, string passwordHash, string email, string role)
    {
        Name = name;
        PasswordHash = passwordHash;
        Email = email;
        UserName = email;
        Role = role;
    }


    // Delete User
    public void Delete()
    {
        Status = UserStatus.Deleted;
    }
}

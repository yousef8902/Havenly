using Havenly.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Havenly.DAL.Database.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            // Primary Key
            builder.HasKey(b => b.BookingID);

            // Precision 
            builder.Property(b => b.TotalPrice)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            // Save Enum as String in SQL Server for readability
            builder.Property(b => b.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            // Foreign Key: Booking -> User (Guest)
            
            builder.HasOne(b => b.Guest)
             .WithMany(u => u.Bookings) 
             .HasForeignKey(b => b.GuestUserID)
             .OnDelete(DeleteBehavior.Restrict);

           

            // Foreign Key: Booking -> Listing
            builder.HasOne(b => b.Listing)
                    .WithMany(l => l.Bookings)
                    .HasForeignKey(b => b.ListingID)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

            builder
                  .HasOne(b => b.Payment)
                  .WithOne(p => p.Booking)
                  .HasForeignKey<Payment>(p => p.BookingID)
                  .OnDelete(DeleteBehavior.Restrict);

            // Overlap Querying

            builder.HasIndex(b => new { b.ListingID, b.CheckIn, b.CheckOut })
                   .HasDatabaseName("IX_Booking_ListingID_CheckIn_CheckOut");
        }
    }
}

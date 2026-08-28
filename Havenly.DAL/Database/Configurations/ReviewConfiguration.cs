using Havenly.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Havenly.DAL.Database.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(r => r.ReviewID);

            builder.Property(r => r.Rating).IsRequired();
            builder.Property(r => r.Comment).HasMaxLength(1000);


            builder.HasOne(r => r.User)
                    .WithMany(u => u.Reviews) 
                    .HasForeignKey(r => r.UserID)
                    .OnDelete(DeleteBehavior.Restrict); 

            
            builder.HasOne(r => r.Booking)
                   .WithMany(b => b.Reviews) 
                   .HasForeignKey(r => r.BookingID)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

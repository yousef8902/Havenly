using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;

//using DAL.Entities;

using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Havenly.BLL.Services.Implementations
{
    public class ReviewServices : IReviewServices
    {
        private readonly IReviewRepository reviewRepository;
        private readonly IBookingRepository bookingRepository;
        private readonly IListingRepository listingRepository;
        private readonly IPropertyRepository propertyRepository;

        public ReviewServices(
            IReviewRepository reviewRepository,
            IBookingRepository bookingRepository,
            IListingRepository listingRepository,
            IPropertyRepository propertyRepository)
        {
            this.reviewRepository = reviewRepository;
            this.bookingRepository = bookingRepository;
            this.listingRepository = listingRepository;
            this.propertyRepository = propertyRepository;
        }
        private async Task<bool> IsUserInBooking(String userId,long bookingId)
        {
            try
            {
                Expression<Func<Booking, bool>> predicate = x => x.BookingID == bookingId && x.GuestUserID == userId;
                var list = await bookingRepository.Find(predicate);
                var x = list.FirstOrDefault();
                if (x == null)
                {
                    Console.WriteLine("info:user not found in booking");
                    return false;
                }
                return true;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                return false;
            }


        }
        private async Task<bool>  updateReviewInProperty(DAL.Entities.Property Entity, int rating)
        {
            try
            {
                Entity.UpdateReview(rating);
                if (Entity == null)
                {
                    Console.WriteLine("info:error in  updateReviewInProperty : cannot find property");
                    return false;
                }
                await propertyRepository.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

        public async Task<bool> CreateReview(string userId, long bookingId, int rating, string comment)
        {
            if (rating < 1 || rating > 5)
                return false;

            var booking = await bookingRepository.GetById(bookingId);
            if (booking is null || booking.GuestUserID != userId)
                return false;

            var listing = await listingRepository.GetById(booking.ListingID);
            if (listing is null)
                return false;

            var property = await propertyRepository.GetById(listing.PropertyID);
            if (property is null || property.OwnerUserID == userId)
                return false;

            var existingReview = await reviewRepository.Get(r => r.UserID == userId && r.PropertyID == property.PropertyID);
            if (existingReview is not null)
                return false;

            var review = new Review();
            review.Create(0, userId, property.PropertyID, rating, comment);
            await reviewRepository.Add(review);
            await updateReviewInProperty(property, rating);
            return true;
        }

        public async Task<bool> RespondToReview(string hostUserId, long reviewId, string response)
        {
            var review = await reviewRepository.GetById(reviewId);
            if (review is null)
                return false;

            var property = await propertyRepository.GetById(review.PropertyID);
            if (property is null || property.OwnerUserID != hostUserId)
                return false;

            return await reviewRepository.RespondToReview(reviewId, response);
        }
    }
}
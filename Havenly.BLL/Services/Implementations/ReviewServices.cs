using System.Threading.Tasks;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;

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

        public async Task<bool> CreateReview(String userId, long bookingId, int rating, string comment)
        {
            if (rating < 1 || rating > 5)
                return false;

            var booking = await bookingRepository.GetById(bookingId);
            if (booking is null)
                return false;

            if (booking.GuestUserID .Equals (userId))
                return false;

            if (booking.Status != BookingStatus.Completed)
                return false;

            var existingReview = await reviewRepository.Get(r => r.UserID .Equals (userId) && r.BookingID == bookingId);
            if (existingReview is not null)
                return false;

            var review = new Review();
            review.Create(0, userId, bookingId, rating, comment);
            await reviewRepository.Add(review);
            return true;
        }

        public async Task<bool> RespondToReview(long hostUserId, long reviewId, string response)
        {
            var review = await reviewRepository.GetById(reviewId);
            if (review is null)
                return false;

            var booking = await bookingRepository.GetById(review.BookingID);
            if (booking is null)
                return false;

            var listing = await listingRepository.GetById(booking.ListingID);
            if (listing is null)
                return false;

            var property = await propertyRepository.GetById(listing.PropertyID);
            if (property is null)
                return false;

            if (!(property.OwnerUserID .Equals( hostUserId)))
                return false;

            return await reviewRepository.RespondToReview(reviewId, response);
        }
    }
}
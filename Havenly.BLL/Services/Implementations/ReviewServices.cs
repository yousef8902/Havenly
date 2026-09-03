using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.EntityFrameworkCore;

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

        private async Task<bool> updateReviewInProperty(Property entity, int rating)
        {
            try
            {
                if (entity == null)
                {
                    return false;
                }
                entity.UpdateReview(rating);
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
            var result = await SubmitReviewAsync(userId, bookingId, rating, comment);
            return result.Success;
        }

        public async Task<ReviewSubmitResultVM> SubmitReviewAsync(string userId, long bookingId, int rating, string comment)
        {
            if (rating < 1 || rating > 5)
            {
                return new ReviewSubmitResultVM { Success = false, ErrorMessage = "Rating must be between 1 and 5 stars." };
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                return new ReviewSubmitResultVM { Success = false, ErrorMessage = "Please write a comment for your review." };
            }

            var booking = await bookingRepository.GetAll()
                .Include(b => b.Guest)
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                        .ThenInclude(p => p.Owner)
                .FirstOrDefaultAsync(b => b.BookingID == bookingId);

            if (booking is null || booking.GuestUserID != userId)
            {
                return new ReviewSubmitResultVM { Success = false, ErrorMessage = "Booking reservation not found or not owned by you." };
            }

            var property = booking.Listing?.Property;
            if (property is null)
            {
                return new ReviewSubmitResultVM { Success = false, ErrorMessage = "Associated property could not be found." };
            }

            if (property.OwnerUserID == userId)
            {
                return new ReviewSubmitResultVM { Success = false, ErrorMessage = "Hosts cannot review their own properties." };
            }

            var existingReview = await reviewRepository.Get(r => r.UserID == userId && r.PropertyID == property.PropertyID);
            if (existingReview is not null)
            {
                return new ReviewSubmitResultVM { Success = false, ErrorMessage = "You have already reviewed this stay." };
            }

            var review = new Review();
            review.Create(0, userId, property.PropertyID, rating, comment.Trim());
            await reviewRepository.Add(review);
            await updateReviewInProperty(property, rating);

            return new ReviewSubmitResultVM
            {
                Success = true,
                ReviewId = review.ReviewID,
                PropertyId = property.PropertyID,
                PropertyName = property.PropertyName,
                HostEmail = property.Owner?.Email ?? "",
                HostName = property.Owner?.Name ?? "Host",
                GuestName = booking.Guest?.Name ?? "Traveler",
                Rating = rating,
                Comment = comment.Trim()
            };
        }

        public async Task<ReviewCreateVM?> GetReviewFormDataAsync(long bookingId, string userId)
        {
            var booking = await bookingRepository.GetAll()
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                        .ThenInclude(p => p.Images)
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                        .ThenInclude(p => p.Address)
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                        .ThenInclude(p => p.Owner)
                .FirstOrDefaultAsync(b => b.BookingID == bookingId);

            if (booking == null || booking.GuestUserID != userId || booking.Listing?.Property == null)
            {
                return null;
            }

            var prop = booking.Listing.Property;
            var hasReviewed = (await reviewRepository.Get(r => r.UserID == userId && r.PropertyID == prop.PropertyID)) != null;
            if (hasReviewed)
            {
                return null;
            }

            var primaryImage = prop.Images?.FirstOrDefault(i => i.IsPrimary == true)?.ImagePath
                               ?? prop.Images?.FirstOrDefault()?.ImagePath
                               ?? "/images/p1.jpg";

            return new ReviewCreateVM
            {
                BookingId = booking.BookingID,
                PropertyId = prop.PropertyID,
                PropertyName = prop.PropertyName,
                PropertyImage = primaryImage,
                City = prop.Address?.City ?? "",
                Country = prop.Address?.Country ?? "Egypt",
                HostName = prop.Owner?.Name ?? "Host",
                HostAvatar = prop.Owner?.ProfilePictureUrl ?? "",
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                Rating = 5,
                Comment = ""
            };
        }

        public async Task<ReviewRespondVM?> GetReviewForResponseAsync(long reviewId, string hostUserId)
        {
            var review = await reviewRepository.GetDetailById(reviewId);
            if (review == null || review.Property?.OwnerUserID != hostUserId)
            {
                return null;
            }

            return new ReviewRespondVM
            {
                ReviewId = review.ReviewID,
                PropertyId = review.PropertyID,
                PropertyName = review.Property.PropertyName,
                GuestName = review.User?.Name ?? "Guest",
                Rating = review.Rating,
                GuestComment = review.Comment,
                Response = review.HostResponse ?? ""
            };
        }

        public async Task<bool> RespondToReview(string hostUserId, long reviewId, string response)
        {
            var review = await reviewRepository.GetDetailById(reviewId);
            if (review is null)
                return false;

            if (review.Property is null || review.Property.OwnerUserID != hostUserId)
                return false;

            return await reviewRepository.RespondToReview(reviewId, response.Trim());
        }

        public async Task<bool> HasGuestReviewedBookingAsync(long bookingId, string userId)
        {
            var booking = await bookingRepository.GetById(bookingId);
            if (booking == null) return false;

            var listing = await listingRepository.GetById(booking.ListingID);
            if (listing == null) return false;

            var review = await reviewRepository.Get(r => r.UserID == userId && r.PropertyID == listing.PropertyID);
            return review != null;
        }
    }
}
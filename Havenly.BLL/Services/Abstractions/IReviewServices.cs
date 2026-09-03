using System.Threading.Tasks;
using Havenly.BLL.ModelVMs;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IReviewServices
    {
        Task<bool> CreateReview(string userId, long bookingId, int rating, string comment);
        Task<ReviewSubmitResultVM> SubmitReviewAsync(string userId, long bookingId, int rating, string comment);
        Task<ReviewCreateVM?> GetReviewFormDataAsync(long bookingId, string userId);
        Task<ReviewRespondVM?> GetReviewForResponseAsync(long reviewId, string hostUserId);
        Task<bool> RespondToReview(string hostUserId, long reviewId, string response);
        Task<bool> HasGuestReviewedBookingAsync(long bookingId, string userId);
    }
}

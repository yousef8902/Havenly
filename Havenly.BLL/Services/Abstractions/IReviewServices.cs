namespace Havenly.BLL.Services.Abstractions
{
    public interface IReviewServices
    {
        Task<bool> CreateReview(String userId, long bookingId, int rating, string comment);
        Task<bool> RespondToReview(long hostUserId, long reviewId, string response);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IReviewServices
    {
        Task<bool> CreateReview(String userId, long bookingId, int rating, string comment);
        Task<bool> RespondToReview(string hostUserId, long reviewId, string response);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IReviewServices
    {
        Task<bool> CreateReview(long userId, long bookingId, int rating, string comment);
        Task<bool> RespondToReview(long hostUserId, long reviewId, string response);
    }
}

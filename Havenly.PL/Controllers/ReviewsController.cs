using Microsoft.AspNetCore.Mvc;
using Havenly.BLL.Services.Abstractions;

namespace Havenly.PL.Controllers
{
    // TODO: SECURITY — no authentication system exists yet.
    // userId/hostUserId are currently passed as raw form values instead of
    // being derived from the logged-in user's session/claims.
    // Must fix once login is implemented, or any user could post as anyone.
    public class ReviewsController : Controller
    {
        private readonly IReviewServices reviewServices;

        public ReviewsController(IReviewServices reviewServices)
        {
            this.reviewServices = reviewServices;
        }

        [HttpPost]
        public async Task<IActionResult> Create(long bookingId, String userId, int rating, string comment)
        {
            var success = await reviewServices.CreateReview(userId, bookingId, rating, comment);
            TempData["Message"] = success
                ? "Review submitted."
                : "Could not submit review. Make sure the stay is completed and hasn't been reviewed yet.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Respond(long reviewId, long hostUserId, string response)
        {
            var success = await reviewServices.RespondToReview(hostUserId, reviewId, response);
            TempData["Message"] = success
                ? "Response posted."
                : "Could not post response. Only the property's host can respond to this review.";
            return RedirectToAction("Index", "Home");
        }
    }
}
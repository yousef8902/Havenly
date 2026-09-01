using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Havenly.BLL.Services.Abstractions;

namespace Havenly.PL.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly IReviewServices reviewServices;

        public ReviewsController(IReviewServices reviewServices)
        {
            this.reviewServices = reviewServices;
        }

        [HttpPost]
        public async Task<IActionResult> Create(long bookingId, int rating, string comment)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Message"] = "You must be logged in to submit a review.";
                return RedirectToAction("Index", "Home");
            }

            var success = await reviewServices.CreateReview(userId, bookingId, rating, comment);
            TempData["Message"] = success
                ? "Review submitted."
                : "Could not submit review. Make sure the stay is completed and hasn't been reviewed yet.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Respond(long reviewId, string response)
        {
            var hostUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(hostUserId))
            {
                TempData["Message"] = "You must be logged in to respond to a review.";
                return RedirectToAction("Index", "Home");
            }

            var success = await reviewServices.RespondToReview(hostUserId, reviewId, response);
            TempData["Message"] = success
                ? "Response posted."
                : "Could not post response. Only the property's host can respond to this review.";
            return RedirectToAction("Index", "Home");
        }
    }
}
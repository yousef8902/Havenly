using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Havenly.PL.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IReviewServices _reviewServices;
        private readonly IEmailServices _emailServices;
        private readonly IBookingRepository _bookingRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public ReviewsController(
            IReviewServices reviewServices,
            IEmailServices emailServices,
            IBookingRepository bookingRepository,
            IReviewRepository reviewRepository,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _reviewServices = reviewServices;
            _emailServices = emailServices;
            _bookingRepository = bookingRepository;
            _reviewRepository = reviewRepository;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Reviews/Create?bookingId=123
        [HttpGet]
        public async Task<IActionResult> Create(long bookingId)
        {
            var booking = await _bookingRepository.GetAll()
                .Include(b => b.Guest)
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                .FirstOrDefaultAsync(b => b.BookingID == bookingId);

            if (booking == null)
            {
                TempData["ErrorMessage"] = "Reservation not found.";
                return RedirectToAction("Index", "Home");
            }

            var guestEmail = booking.Guest?.Email ?? string.Empty;
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // If not logged in, redirect to login pre-filling the guest's email
            if (string.IsNullOrEmpty(currentUserId))
            {
                return RedirectToAction("Login", "Account", new {
                    returnUrl = Url.Action("Create", "Reviews", new { bookingId }),
                    email = guestEmail
                });
            }

            // If logged in as another account (e.g. host), sign out and redirect to login to switch accounts
            if (currentUserId != booking.GuestUserID)
            {
                var currentUser = await _userManager.FindByIdAsync(currentUserId);
                var currentEmail = currentUser?.Email ?? "another account";

                TempData["InfoMessage"] = $"You were signed in as {currentEmail}. This review link belongs to {guestEmail}. Please log in to your traveler account to review your stay.";

                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account", new {
                    returnUrl = Url.Action("Create", "Reviews", new { bookingId }),
                    email = guestEmail
                });
            }

            // Check if already reviewed
            var hasReviewed = await _reviewServices.HasGuestReviewedBookingAsync(bookingId, currentUserId);
            if (hasReviewed)
            {
                TempData["InfoMessage"] = "You have already submitted a review for this stay. Thank you for your valuable feedback!";
                return RedirectToAction("Detail", "Property", new { id = booking.Listing?.PropertyID });
            }

            var model = await _reviewServices.GetReviewFormDataAsync(bookingId, currentUserId);
            if (model == null)
            {
                TempData["ErrorMessage"] = "You cannot review this stay. The reservation must be completed and not already reviewed.";
                return RedirectToAction("MyBookings", "Booking");
            }

            return View(model);
        }

        // POST: /Reviews/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewCreateVM model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _reviewServices.SubmitReviewAsync(userId, model.BookingId, model.Rating, model.Comment);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Could not submit review.");
                return View(model);
            }

            // Notify Host via Email
            try
            {
                if (!string.IsNullOrEmpty(result.HostEmail))
                {
                    await _emailServices.SendReviewNotificationToHostAsync(
                        result.HostEmail,
                        result.HostName,
                        result.GuestName,
                        result.PropertyName,
                        result.Rating,
                        result.Comment,
                        result.ReviewId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HOST REVIEW NOTIFICATION ERROR] {ex.Message}");
            }

            TempData["SuccessMessage"] = "Thank you! Your review has been published and your host has been notified.";
            return RedirectToAction("Detail", "Property", new { id = result.PropertyId });
        }

        // GET: /Reviews/Respond?reviewId=456
        [HttpGet]
        public async Task<IActionResult> Respond(long reviewId)
        {
            var review = await _reviewRepository.GetDetailById(reviewId);
            if (review == null || review.Property == null)
            {
                TempData["ErrorMessage"] = "Review not found.";
                return RedirectToAction("Index", "Home");
            }

            var hostUserId = review.Property.OwnerUserID;
            var hostUser = await _userManager.FindByIdAsync(hostUserId);
            var hostEmail = hostUser?.Email ?? string.Empty;

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // If not logged in, redirect to login pre-filling the host's email
            if (string.IsNullOrEmpty(currentUserId))
            {
                return RedirectToAction("Login", "Account", new {
                    returnUrl = Url.Action("Respond", "Reviews", new { reviewId }),
                    email = hostEmail
                });
            }

            // If logged in as another account (e.g. guest), sign out and redirect to login to switch accounts
            if (currentUserId != hostUserId)
            {
                var currentUser = await _userManager.FindByIdAsync(currentUserId);
                var currentEmail = currentUser?.Email ?? "another account";

                TempData["InfoMessage"] = $"You were signed in as {currentEmail}. This listing is hosted by {hostEmail}. Please log in to your host account to post an official reply.";

                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account", new {
                    returnUrl = Url.Action("Respond", "Reviews", new { reviewId }),
                    email = hostEmail
                });
            }

            var model = await _reviewServices.GetReviewForResponseAsync(reviewId, currentUserId);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Review not found or you are not authorized to respond to this review.";
                return RedirectToAction("Bookings", "Host");
            }

            return View(model);
        }

        // POST: /Reviews/Respond
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Respond(ReviewRespondVM model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _reviewServices.RespondToReview(userId, model.ReviewId, model.Response);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Could not post response. Only the property host can reply to this review.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Your response has been published and is now visible to all guests on your listing page!";
            return RedirectToAction("Detail", "Property", new { id = model.PropertyId });
        }
    }
}
using System.Security.Claims;
using Havenly.BLL.ModelVMs;
using Havenly.BLL.ModelVMs.Payment;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Havenly.PL.Controllers
{
    [Authorize(Roles = $"{UserRoles.Host},{UserRoles.Admin}")]
    public class HostController : Controller
    {
        private readonly IPropertyServices _propertyServices;
        private readonly IAmenityRepository _amenityRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingService _bookingService;
        private readonly IReviewRepository _reviewRepository;
        private readonly IPaymentService _paymentService;
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailServices _emailServices;
        private readonly UserManager<User> _userManager;

        public HostController(
            IPropertyServices propertyServices,
            IAmenityRepository amenityRepository,
            IBookingRepository bookingRepository,
            IBookingService bookingService,
            IReviewRepository reviewRepository,
            IPaymentService paymentService,
            IWebHostEnvironment environment,
            IEmailServices emailServices,
            UserManager<User> userManager)
        {
            _propertyServices = propertyServices;
            _amenityRepository = amenityRepository;
            _bookingRepository = bookingRepository;
            _bookingService = bookingService;
            _reviewRepository = reviewRepository;
            _paymentService = paymentService;
            _environment = environment;
            _emailServices = emailServices;
            _userManager = userManager;
        }

        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        public override async Task OnActionExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context, Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate next)
        {
            if (!User.IsInRole(UserRoles.Admin))
            {
                var userId = GetCurrentUserId();
                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null && user.Status == UserStatus.PendingApproval)
                    {
                        context.Result = RedirectToAction("HostPendingApproval", "Account", new { email = user.Email });
                        return;
                    }
                }
            }

            await base.OnActionExecutionAsync(context, next);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var properties = (await _propertyServices.GetPropertiesByOwner(userId)).ToList();
            var activeCount = properties.Count(p => p.Listing != null && p.Listing.ListingStatus == ListingStatus.Approved);
            var pendingCount = properties.Count(p => p.Listing != null && p.Listing.ListingStatus == ListingStatus.Pending);

            ViewBag.ActiveListings = activeCount;
            ViewBag.PendingListings = pendingCount;
            ViewBag.TotalListings = properties.Count;

            var payoutsData = await _paymentService.GetHostPayoutsAsync(userId);
            ViewBag.TotalEarnings = payoutsData.TotalNetEarnings;
            ViewBag.PendingPayouts = payoutsData.PendingPayouts;

            var propertyIds = properties.Select(p => p.PropertyID).ToHashSet();
            var allBookings = await _bookingRepository.GetAll()
                .Include(b => b.Guest)
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                        .ThenInclude(p => p.Address)
                .ToListAsync();

            var hostBookings = allBookings
                .Where(b => b.Listing != null && propertyIds.Contains(b.Listing.PropertyID))
                .OrderByDescending(b => b.CheckIn)
                .Take(5)
                .ToList();

            ViewBag.RecentBookings = hostBookings;

            var unrepliedReviews = (await _reviewRepository.Find(r => propertyIds.Contains(r.PropertyID) && string.IsNullOrEmpty(r.HostResponse))).ToList();
            ViewBag.UnrepliedReviewsCount = unrepliedReviews.Count;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListProperties()
        {
            var userId = GetCurrentUserId();
            var properties = await _propertyServices.GetPropertiesByOwner(userId);

            var cardList = new List<HostPropertyCardVM>();
            foreach (var p in properties)
            {
                var upcomingCount = await _propertyServices.GetUpcomingBookingsCount(p.PropertyID, userId);
                cardList.Add(new HostPropertyCardVM
                {
                    PropertyId = p.PropertyID,
                    ListingId = p.Listing?.ListingID ?? 0,
                    PropertyName = p.PropertyName,
                    Category = p.Category ?? "Design homes",
                    City = p.Address?.City ?? string.Empty,
                    Country = p.Address?.Country ?? string.Empty,
                    Price = p.Listing?.Price ?? 0,
                    Guests = p.NumberOfGuests,
                    ListingStatus = p.Listing?.ListingStatus ?? ListingStatus.Pending,
                    IsValid = p.Listing?.IsValid ?? false,
                    UpcomingBookingsCount = upcomingCount,
                    ImageUrl = p.Images?.FirstOrDefault(img => img.IsPrimary == true)?.ImagePath
                               ?? p.Images?.FirstOrDefault()?.ImagePath
                               ?? "/images/p1.jpg",
                    Rating = p.Rating,
                    NumberOfReviews = p.NumberOfReviews
                });
            }

            var vm = new HostPropertiesPageVM
            {
                Properties = cardList
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProperty()
        {
            var model = new HostPropertyFormVM();
            await PopulateAvailableAmenities(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProperty(HostPropertyFormVM model, List<IFormFile>? imageFiles)
        {
            if (!ModelState.IsValid)
            {
                await PopulateAvailableAmenities(model);
                return View(model);
            }

            var userId = GetCurrentUserId();

            var property = new Property();
            property.Create(
                userId,
                0,
                model.PropertyName,
                model.Description,
                model.NumberOfGuests,
                model.Capacity,
                model.BathroomCount,
                model.Category);

            var address = new Address();
            address.Create(0, model.Country, model.City, model.Street, 0m, 0m);

            var listing = new Listing();
            listing.Create(0, model.Description, model.Price);
            listing.ListingStatus = ListingStatus.Pending;

            var propertyAmenities = (model.SelectedAmenityIds ?? new List<long>())
                .Select(amenityId =>
                {
                    var pa = new PropertyAmenity();
                    pa.Create(0, amenityId);
                    return pa;
                }).ToList();

            var bedrooms = new List<Bedroom>();
            if (model.Bedrooms != null && model.Bedrooms.Count > 0)
            {
                int roomNum = 1;
                foreach (var bInput in model.Bedrooms)
                {
                    var bedCount = bInput.BedQuantity > 0 ? bInput.BedQuantity : 1;
                    var bedroom = new Bedroom();
                    bedroom.Create(0, 0, roomNum++, bedcnt: bedCount, roomName: string.IsNullOrWhiteSpace(bInput.RoomName) ? $"Bedroom {roomNum - 1}" : bInput.RoomName);

                    var bed = new Bed();
                    bed.Create(0, 0, bInput.BedType, bedCount);
                    bedroom.Beds = new List<Bed> { bed };

                    bedrooms.Add(bedroom);
                }
            }
            else
            {
                var defaultBedroom = new Bedroom();
                defaultBedroom.Create(0, 0, 1, bedcnt: 1, roomName: "Master Bedroom");
                var defaultBed = new Bed();
                defaultBed.Create(0, 0, BedType.Double, 1);
                defaultBedroom.Beds = new List<Bed> { defaultBed };
                bedrooms.Add(defaultBedroom);
            }

            var success = await _propertyServices.CreateProperty(
                property,
                address,
                listing,
                propertyAmenities,
                bedrooms,
                new List<PropertyImage>());

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Unable to create property. Please verify the submitted details.");
                await PopulateAvailableAmenities(model);
                return View(model);
            }

            if (imageFiles != null && imageFiles.Count > 0)
            {
                var savedImages = await SaveUploadedImages(property.PropertyID, imageFiles);
                if (savedImages.Count > 0)
                {
                    await _propertyServices.AddPropertyImages(property.PropertyID, userId, savedImages);
                }
            }

            // Notify Admins of new listing
            try
            {
                var hostUser = await _userManager.FindByIdAsync(userId);
                var admins = await _userManager.GetUsersInRoleAsync(UserRoles.Admin);
                var adminEmails = admins.Select(a => a.Email).Where(e => !string.IsNullOrEmpty(e)).Distinct().ToList();
                if (!adminEmails.Any())
                {
                    adminEmails.Add("admin@test.com");
                }

                foreach (var adminEmail in adminEmails)
                {
                    await _emailServices.SendListingSubmittedToAdminAsync(
                        adminEmail!,
                        hostUser?.Name ?? "Host",
                        hostUser?.Email ?? "N/A",
                        model.PropertyName,
                        model.Category,
                        $"{model.City}, {model.Country}",
                        model.Price,
                        property.PropertyID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ADMIN NOTIFICATION ERROR] {ex.Message}");
            }

            TempData["SuccessMessage"] = "Property submitted successfully! It is currently pending review by an admin.";
            return RedirectToAction(nameof(ListProperties));
        }

        [HttpGet]
        public async Task<IActionResult> EditProperty(long id)
        {
            var userId = GetCurrentUserId();
            var property = await _propertyServices.GetPropertyDetails(id, userId);

            if (property == null || property.OwnerUserID != userId || property.IsDeleted)
            {
                return NotFound();
            }

            var model = new HostPropertyFormVM
            {
                PropertyId = property.PropertyID,
                PropertyName = property.PropertyName,
                Category = property.Category ?? "Design homes",
                Description = property.Description,
                Country = property.Address?.Country ?? string.Empty,
                City = property.Address?.City ?? string.Empty,
                Street = property.Address?.Street ?? string.Empty,
                Price = property.Listing?.Price ?? 0,
                NumberOfGuests = property.NumberOfGuests,
                Capacity = property.Capacity,
                BathroomCount = property.BathroomCount,
                SelectedAmenityIds = property.PropertyAmenities?.Select(pa => pa.AmenitiesID).ToList() ?? new List<long>(),
                ExistingImages = property.Images?.Select(img => new PropertyImageItemVM
                {
                    ImageId = img.ImageID,
                    ImagePath = img.ImagePath,
                    IsPrimary = img.IsPrimary == true
                }).ToList() ?? new List<PropertyImageItemVM>()
            };

            await PopulateAvailableAmenities(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProperty(long id, HostPropertyFormVM model, List<IFormFile>? newImageFiles)
        {
            if (!ModelState.IsValid)
            {
                await PopulateAvailableAmenities(model);
                return View(model);
            }

            var userId = GetCurrentUserId();

            var property = new Property();
            property.Create(
                userId,
                0,
                model.PropertyName,
                model.Description,
                model.NumberOfGuests,
                model.Capacity,
                model.BathroomCount,
                model.Category);

            var address = new Address();
            address.Create(0, model.Country, model.City, model.Street, 0m, 0m);

            var listing = new Listing();
            listing.Create(0, model.Description, model.Price);

            var propertyAmenities = (model.SelectedAmenityIds ?? new List<long>())
                .Select(amenityId =>
                {
                    var pa = new PropertyAmenity();
                    pa.Create(0, amenityId);
                    return pa;
                }).ToList();

            var bedrooms = new List<Bedroom>();
            if (model.Bedrooms != null && model.Bedrooms.Count > 0)
            {
                int roomNum = 1;
                foreach (var bInput in model.Bedrooms)
                {
                    var bedCount = bInput.BedQuantity > 0 ? bInput.BedQuantity : 1;
                    var bedroom = new Bedroom();
                    bedroom.Create(0, 0, roomNum++, bedcnt: bedCount, roomName: string.IsNullOrWhiteSpace(bInput.RoomName) ? $"Bedroom {roomNum - 1}" : bInput.RoomName);

                    var bed = new Bed();
                    bed.Create(0, 0, bInput.BedType, bedCount);
                    bedroom.Beds = new List<Bed> { bed };

                    bedrooms.Add(bedroom);
                }
            }

            var success = await _propertyServices.UpdateProperty(
                id,
                userId,
                property,
                address,
                listing,
                propertyAmenities,
                bedrooms);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Unable to update property.");
                await PopulateAvailableAmenities(model);
                return View(model);
            }

            if (newImageFiles != null && newImageFiles.Count > 0)
            {
                var savedImages = await SaveUploadedImages(id, newImageFiles);
                if (savedImages.Count > 0)
                {
                    await _propertyServices.AddPropertyImages(id, userId, savedImages);
                }
            }

            TempData["SuccessMessage"] = "Property updated and submitted for admin re-approval.";
            return RedirectToAction(nameof(ListProperties));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePrice(long propertyId, decimal price)
        {
            var userId = GetCurrentUserId();
            if (price <= 0)
            {
                TempData["ErrorMessage"] = "Nightly price must be greater than 0 EGP.";
                return RedirectToAction(nameof(ListProperties));
            }

            var success = await _propertyServices.UpdateListingPrice(propertyId, userId, price);
            if (success)
            {
                TempData["SuccessMessage"] = $"Nightly price updated successfully to {price:N0} EGP.";
            }
            else
            {
                TempData["ErrorMessage"] = "Could not update property price.";
            }

            return RedirectToAction(nameof(ListProperties));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleListingStatus(long propertyId)
        {
            var userId = GetCurrentUserId();
            var (success, isActive, message) = await _propertyServices.ToggleListingSuspension(propertyId, userId);

            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(ListProperties));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProperty(long id)
        {
            var userId = GetCurrentUserId();
            var upcomingCount = await _propertyServices.GetUpcomingBookingsCount(id, userId);

            var success = await _propertyServices.DeleteProperty(id, userId);

            if (success)
            {
                if (upcomingCount > 0)
                {
                    TempData["SuccessMessage"] = $"Property unlisted and removed from search. Note: You have {upcomingCount} upcoming confirmed reservation(s) which remain active and honored for your guests.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Property unlisted and deleted successfully.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Could not delete property.";
            }

            return RedirectToAction(nameof(ListProperties));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(long propertyId, long imageId)
        {
            var userId = GetCurrentUserId();
            var success = await _propertyServices.RemovePropertyImage(propertyId, imageId, userId);
            if (success)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> Bookings()
        {
            await _bookingService.ProcessAutomaticCheckoutsAsync();

            var userId = GetCurrentUserId();
            var properties = (await _propertyServices.GetPropertiesByOwner(userId)).ToList();
            var propertyIds = properties.Select(p => p.PropertyID).ToHashSet();

            var allBookings = await _bookingRepository.GetAll()
                .Include(b => b.Guest)
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                        .ThenInclude(p => p.Address)
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                        .ThenInclude(p => p.Reviews)
                .ToListAsync();

            var hostBookings = allBookings
                .Where(b => b.Listing != null && propertyIds.Contains(b.Listing.PropertyID))
                .OrderByDescending(b => b.CheckIn)
                .ToList();

            return View(hostBookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptBooking(long id)
        {
            var userId = GetCurrentUserId();
            var properties = (await _propertyServices.GetPropertiesByOwner(userId)).ToList();
            var propertyIds = properties.Select(p => p.PropertyID).ToHashSet();

            var booking = await _bookingRepository.GetAll()
                .Include(b => b.Guest)
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                        .ThenInclude(p => p.Address)
                .FirstOrDefaultAsync(b => b.BookingID == id);

            if (booking == null || booking.Listing == null || !propertyIds.Contains(booking.Listing.PropertyID))
            {
                TempData["ErrorMessage"] = "Booking not found or you do not have permission to manage this booking.";
                return RedirectToAction(nameof(Bookings));
            }

            booking.UpdateStatus(BookingStatus.Approved);
            _bookingRepository.Update(booking);
            await _bookingRepository.SaveChanges();

            // Send confirmation email to Guest
            try
            {
                var guest = booking.Guest ?? await _userManager.FindByIdAsync(booking.GuestUserID);
                var host = await _userManager.FindByIdAsync(userId);
                var prop = properties.FirstOrDefault(p => p.PropertyID == booking.Listing.PropertyID) ?? booking.Listing.Property;

                if (guest != null && !string.IsNullOrEmpty(guest.Email))
                {
                    await _emailServices.SendBookingStatusToGuestAsync(
                        guest.Email,
                        guest.Name ?? "Guest",
                        host?.Name ?? "Host",
                        prop?.PropertyName ?? "Havenly Stay",
                        booking.CheckIn,
                        booking.CheckOut,
                        booking.TotalPrice,
                        prop?.Address?.City ?? "",
                        prop?.Address?.Country ?? "",
                        true,
                        booking.BookingID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GUEST ACCEPT NOTICE ERROR] {ex.Message}");
            }

            TempData["SuccessMessage"] = $"Booking #{id} has been accepted and approved.";
            return RedirectToAction(nameof(Bookings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectBooking(long id)
        {
            var userId = GetCurrentUserId();
            var properties = (await _propertyServices.GetPropertiesByOwner(userId)).ToList();
            var propertyIds = properties.Select(p => p.PropertyID).ToHashSet();

            var booking = await _bookingRepository.GetAll()
                .Include(b => b.Guest)
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                        .ThenInclude(p => p.Address)
                .FirstOrDefaultAsync(b => b.BookingID == id);

            if (booking == null || booking.Listing == null || !propertyIds.Contains(booking.Listing.PropertyID))
            {
                TempData["ErrorMessage"] = "Booking not found or you do not have permission to manage this booking.";
                return RedirectToAction(nameof(Bookings));
            }

            booking.UpdateStatus(BookingStatus.Rejected);
            _bookingRepository.Update(booking);
            await _bookingRepository.SaveChanges();

            // Send rejection email to Guest
            try
            {
                var guest = booking.Guest ?? await _userManager.FindByIdAsync(booking.GuestUserID);
                var host = await _userManager.FindByIdAsync(userId);
                var prop = properties.FirstOrDefault(p => p.PropertyID == booking.Listing.PropertyID) ?? booking.Listing.Property;

                if (guest != null && !string.IsNullOrEmpty(guest.Email))
                {
                    await _emailServices.SendBookingStatusToGuestAsync(
                        guest.Email,
                        guest.Name ?? "Guest",
                        host?.Name ?? "Host",
                        prop?.PropertyName ?? "Havenly Stay",
                        booking.CheckIn,
                        booking.CheckOut,
                        booking.TotalPrice,
                        prop?.Address?.City ?? "",
                        prop?.Address?.Country ?? "",
                        false,
                        booking.BookingID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GUEST REJECT NOTICE ERROR] {ex.Message}");
            }

            TempData["SuccessMessage"] = $"Booking #{id} has been rejected.";
            return RedirectToAction(nameof(Bookings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteStay(long id)
        {
            var hostUserId = GetCurrentUserId();
            var booking = await _bookingRepository.GetAll()
                .Include(b => b.Guest)
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                .FirstOrDefaultAsync(b => b.BookingID == id);

            if (booking == null || booking.Listing?.Property?.OwnerUserID != hostUserId)
            {
                TempData["ErrorMessage"] = "Booking not found or you do not have permission to manage this reservation.";
                return RedirectToAction(nameof(Bookings));
            }

            if (booking.Status != BookingStatus.Approved)
            {
                TempData["ErrorMessage"] = "Only approved stays can be completed.";
                return RedirectToAction(nameof(Bookings));
            }

            booking.UpdateStatus(BookingStatus.Completed);
            await _bookingRepository.SaveChanges();

            // Send automated review invitation to the guest
            try
            {
                if (booking.Guest != null && !string.IsNullOrEmpty(booking.Guest.Email))
                {
                    var hostUser = await _userManager.FindByIdAsync(hostUserId);
                    var hostName = hostUser?.Name ?? "Your Host";
                    var propName = booking.Listing?.Property?.PropertyName ?? "your stay";

                    await _emailServices.SendReviewInvitationToGuestAsync(
                        booking.Guest.Email,
                        booking.Guest.Name ?? "Traveler",
                        hostName,
                        propName,
                        booking.BookingID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REVIEW INVITATION EMAIL ERROR] {ex.Message}");
            }

            TempData["SuccessMessage"] = $"Stay #{id} has been marked as Completed! An email invitation has been sent to the guest to rate and review their stay.";
            return RedirectToAction(nameof(Bookings));
        }

        [HttpGet]
        public async Task<IActionResult> Payouts()
        {
            var userId = GetCurrentUserId();
            var model = await _paymentService.GetHostPayoutsAsync(userId);
            return View(model);
        }

        private async Task PopulateAvailableAmenities(HostPropertyFormVM model)
        {
            var allAmenities = await _amenityRepository.GetAll();
            var selectedSet = (model.SelectedAmenityIds ?? new List<long>()).ToHashSet();

            model.AvailableAmenities = allAmenities.Select(a => new AmenityOptionVM
            {
                AmenityId = a.AmenitiesID,
                Name = a.Name,
                IsSelected = selectedSet.Contains(a.AmenitiesID)
            }).ToList();
        }

        private async Task<List<PropertyImage>> SaveUploadedImages(long propertyId, List<IFormFile> files)
        {
            var imageEntities = new List<PropertyImage>();
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "properties");
            Directory.CreateDirectory(uploadsFolder);

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            const long maxFileSize = 5 * 1024 * 1024; // 5MB

            bool isFirst = true;
            foreach (var file in files)
            {
                if (file.Length == 0 || file.Length > maxFileSize)
                    continue;

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                    continue;

                var uniqueFileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var image = new PropertyImage();
                image.Create(0, propertyId, $"/uploads/properties/{uniqueFileName}");
                image.IsPrimary = isFirst;
                isFirst = false;

                imageEntities.Add(image);
            }

            return imageEntities;
        }
    }
}

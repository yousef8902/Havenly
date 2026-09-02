using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Havenly.BLL.ModelVMs;
using Havenly.BLL.ModelVMs.Account;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Havenly.BLL.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailServices _emailServices;
        private readonly IBookingRepository _bookingRepo;
        private readonly IPropertyRepository _propertyRepo;
        private readonly IFavoriteRepository _favoriteRepo;
        private readonly IReviewRepository _reviewRepo;

        public AccountService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailServices emailServices,
            IBookingRepository bookingRepo,
            IPropertyRepository propertyRepo,
            IFavoriteRepository favoriteRepo,
            IReviewRepository reviewRepo)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailServices = emailServices;
            _bookingRepo = bookingRepo;
            _propertyRepo = propertyRepo;
            _favoriteRepo = favoriteRepo;
            _reviewRepo = reviewRepo;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterVM model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                if (existingUser.EmailConfirmed)
                {
                    return IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = "DuplicateEmail",
                            Description = "Email is already registered."
                        });
                }

                // If user was never confirmed, clean up the stale record so they can re-register freshly
                await _userManager.DeleteAsync(existingUser);
            }

            var roleToAssign = string.Equals(model.Role, UserRoles.Host, StringComparison.OrdinalIgnoreCase) 
                ? UserRoles.Host 
                : UserRoles.Guest;

            var initialStatus = roleToAssign == UserRoles.Host 
                ? UserStatus.PendingApproval 
                : UserStatus.Active;

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                UserName = model.Email,
                Role = roleToAssign,
                Status = initialStatus,
                VerificationDocumentUrl = model.VerificationDocumentUrl,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return result;

            var roleResult = await _userManager.AddToRoleAsync(user, roleToAssign);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return roleResult;
            }

            // Generate 6-digit numeric OTP code
            var otpCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(15).ToString("o");

            await _userManager.SetAuthenticationTokenAsync(user, "Havenly", "EmailVerificationOTP", otpCode);
            await _userManager.SetAuthenticationTokenAsync(user, "Havenly", "EmailVerificationExpiry", expiry);

            // Send confirmation email asynchronously
            await _emailServices.SendEmailVerificationOtpAsync(user.Email!, user.Name, otpCode);

            return IdentityResult.Success;
        }

        public async Task<SignInResult> LoginAsync(LoginVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && !user.EmailConfirmed)
            {
                // Check password validity first
                var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
                if (passwordValid)
                {
                    return SignInResult.NotAllowed;
                }
            }

            return await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);
        }

        public async Task<(bool Succeeded, string? ErrorMessage)> VerifyOtpAsync(string email, string otpCode)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otpCode))
                return (false, "Please provide email and verification code.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "Account not found.");

            if (user.EmailConfirmed)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return (true, null);
            }

            var storedOtp = await _userManager.GetAuthenticationTokenAsync(user, "Havenly", "EmailVerificationOTP");
            var storedExpiryStr = await _userManager.GetAuthenticationTokenAsync(user, "Havenly", "EmailVerificationExpiry");

            if (string.IsNullOrEmpty(storedOtp) || storedOtp != otpCode.Trim())
            {
                return (false, "Invalid verification code. Please check and try again.");
            }

            if (DateTime.TryParse(storedExpiryStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expiryDate))
            {
                if (DateTime.UtcNow > expiryDate)
                {
                    return (false, "Verification code has expired. Please request a new code.");
                }
            }

            // Mark email confirmed
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            // Clean up tokens
            await _userManager.RemoveAuthenticationTokenAsync(user, "Havenly", "EmailVerificationOTP");
            await _userManager.RemoveAuthenticationTokenAsync(user, "Havenly", "EmailVerificationExpiry");

            // Sign user in if active
            if (user.Status != UserStatus.PendingApproval)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return (true, null);
        }

        public async Task<(bool Succeeded, string? ErrorMessage)> ResendOtpAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, "Please provide an email address.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "Account not found.");

            if (user.EmailConfirmed)
                return (false, "This email is already verified.");

            var otpCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(15).ToString("o");

            await _userManager.SetAuthenticationTokenAsync(user, "Havenly", "EmailVerificationOTP", otpCode);
            await _userManager.SetAuthenticationTokenAsync(user, "Havenly", "EmailVerificationExpiry", expiry);

            await _emailServices.SendEmailVerificationOtpAsync(user.Email!, user.Name, otpCode);

            return (true, null);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<UserProfileVM?> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var bookings = await _bookingRepo.GetAll()
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                        .ThenInclude(p => p.Address)
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                        .ThenInclude(p => p.Images)
                .Where(b => b.GuestUserID == userId)
                .OrderByDescending(b => b.CheckIn)
                .ToListAsync();

            var totalFavorites = await _favoriteRepo.GetAll();
            var userFavoritesCount = totalFavorites.Count(f => f.UserID == userId);

            var totalReviews = await _reviewRepo.GetAll();
            var userReviewsCount = totalReviews.Count(r => r.UserID == userId);

            var recentBookingsVm = bookings.Take(5).Select(b => new BookingDetailsVM
            {
                BookingId = b.BookingID,
                PropertyId = b.Listing?.PropertyID ?? 0,
                Title = b.Listing?.Property?.PropertyName ?? $"Property #{b.ListingID}",
                City = b.Listing?.Property?.Address?.City ?? string.Empty,
                Country = b.Listing?.Property?.Address?.Country ?? string.Empty,
                ImageUrl = b.Listing?.Property?.Images?.FirstOrDefault(img => img.IsPrimary == true)?.ImagePath 
                        ?? b.Listing?.Property?.Images?.FirstOrDefault()?.ImagePath 
                        ?? "/images/p1.jpg",
                CheckIn = b.CheckIn,
                CheckOut = b.CheckOut,
                Guests = b.Listing?.Property?.NumberOfGuests ?? 1,
                TotalPrice = b.TotalPrice,
                Status = b.Status.ToString()
            }).ToList();

            return new UserProfileVM
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Bio = user.Bio,
                JoinedDate = user.JoinedDate,
                Role = user.Role,
                Status = user.Status.ToString(),
                TotalBookings = bookings.Count,
                UpcomingBookings = bookings.Count(b => b.Status == BookingStatus.Approved && b.CheckIn >= DateTime.Today),
                CompletedBookings = bookings.Count(b => b.Status == BookingStatus.Completed || (b.Status == BookingStatus.Approved && b.CheckOut < DateTime.Today)),
                TotalFavorites = userFavoritesCount,
                TotalReviews = userReviewsCount,
                RecentBookings = recentBookingsVm
            };
        }

        public async Task<EditProfileVM?> GetEditProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            return new EditProfileVM
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                CurrentProfilePictureUrl = user.ProfilePictureUrl
            };
        }

        public async Task<(bool Succeeded, string? ErrorMessage)> UpdateProfileAsync(string userId, EditProfileVM model, string? savedAvatarPath = null)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return (false, "User not found.");

            user.Name = model.Name;
            user.PhoneNumber = model.PhoneNumber;
            user.Bio = model.Bio;

            if (!string.IsNullOrEmpty(savedAvatarPath))
            {
                user.ProfilePictureUrl = savedAvatarPath;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return (true, null);
        }

        public async Task<PublicUserProfileVM?> GetPublicProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.Status == UserStatus.PendingApproval) return null;

            var properties = (await _propertyRepo.GetByOwner(userId)).ToList();
            var propertyCards = properties.Select(p => new PublicPropertyCardVM
            {
                PropertyId = p.PropertyID,
                ListingId = p.Listing?.ListingID ?? 0,
                PropertyName = p.PropertyName,
                Category = p.Category ?? "Design homes",
                City = p.Address?.City ?? string.Empty,
                Country = p.Address?.Country ?? string.Empty,
                Price = p.Listing?.Price ?? 0,
                Rating = p.Rating,
                NumberOfReviews = (int)p.NumberOfReviews,
                ImageUrl = p.Images?.FirstOrDefault(i => i.IsPrimary == true)?.ImagePath
                        ?? p.Images?.FirstOrDefault()?.ImagePath
                        ?? "/images/p1.jpg"
            }).ToList();

            var propIds = properties.Select(p => p.PropertyID).ToHashSet();
            var allReviews = await _reviewRepo.GetAll();
            var reviewsReceived = allReviews
                .Where(r => propIds.Contains(r.PropertyID))
                .OrderByDescending(r => r.ReviewID)
                .Take(6)
                .Select(r => new PublicUserReviewVM
                {
                    ReviewerName = r.User?.Name ?? "Guest",
                    ReviewerAvatar = r.User?.ProfilePictureUrl,
                    PropertyName = r.Property?.PropertyName ?? "Stay",
                    Rating = r.Rating,
                    Comment = r.Comment ?? string.Empty,
                    CreatedAt = DateTime.UtcNow.AddDays(-14)
                }).ToList();

            var allBookings = await _bookingRepo.GetAll()
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                        .ThenInclude(p => p.Address)
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                        .ThenInclude(p => p.Images)
                .Where(b => b.GuestUserID == userId)
                .OrderByDescending(b => b.CheckIn)
                .ToListAsync();

            var completedStays = allBookings.Count(b => b.Status == BookingStatus.Completed || b.Status == BookingStatus.Approved);

            var guestStays = allBookings
                .Take(6)
                .Select(b => new PublicGuestStayVM
                {
                    BookingId = b.BookingID,
                    PropertyId = b.Listing?.Property?.PropertyID ?? 0,
                    PropertyName = b.Listing?.Property?.PropertyName ?? $"Stay #{b.ListingID}",
                    City = b.Listing?.Property?.Address?.City ?? string.Empty,
                    Country = b.Listing?.Property?.Address?.Country ?? "Egypt",
                    ImageUrl = b.Listing?.Property?.Images?.FirstOrDefault(i => i.IsPrimary == true)?.ImagePath
                            ?? b.Listing?.Property?.Images?.FirstOrDefault()?.ImagePath
                            ?? "/images/p1.jpg",
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    Status = b.Status.ToString()
                }).ToList();

            List<PublicUserReviewVM> displayReviews;
            if (string.Equals(user.Role, UserRoles.Host, StringComparison.OrdinalIgnoreCase))
            {
                displayReviews = reviewsReceived;
            }
            else
            {
                displayReviews = allReviews
                    .Where(r => r.UserID == userId)
                    .OrderByDescending(r => r.ReviewID)
                    .Take(6)
                    .Select(r => new PublicUserReviewVM
                    {
                        ReviewerName = user.Name ?? "Traveler",
                        ReviewerAvatar = user.ProfilePictureUrl,
                        PropertyName = r.Property?.PropertyName ?? "Egypt Stay",
                        Rating = r.Rating,
                        Comment = r.Comment ?? string.Empty,
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    }).ToList();
            }

            double avgRating = properties.Any(p => p.NumberOfReviews > 0)
                ? Math.Round(properties.Where(p => p.NumberOfReviews > 0).Average(p => p.Rating), 2)
                : 5.0;

            return new PublicUserProfileVM
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Bio = user.Bio,
                JoinedDate = user.JoinedDate,
                Role = user.Role,
                Status = user.Status.ToString(),
                IsEmailConfirmed = user.EmailConfirmed,
                IsPhoneConfirmed = !string.IsNullOrEmpty(user.PhoneNumber),
                TotalProperties = properties.Count,
                TotalCompletedStays = completedStays,
                TotalReviewsReceived = reviewsReceived.Count,
                AverageHostRating = avgRating,
                Properties = propertyCards,
                GuestStays = guestStays,
                Reviews = displayReviews
            };
        }

        public async Task<(bool Succeeded, string? ErrorMessage)> SendPasswordResetOtpAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, "Please provide an email address.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "No account found with this email address.");

            var otpCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(15).ToString("o");

            await _userManager.SetAuthenticationTokenAsync(user, "Havenly", "PasswordResetOTP", otpCode);
            await _userManager.SetAuthenticationTokenAsync(user, "Havenly", "PasswordResetExpiry", expiry);

            await _emailServices.SendPasswordResetOtpAsync(user.Email!, user.Name, otpCode);

            return (true, null);
        }

        public async Task<(bool Succeeded, string? Token, string? ErrorMessage)> VerifyPasswordResetOtpAsync(string email, string otpCode)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otpCode))
                return (false, null, "Please provide email and verification code.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, null, "Account not found.");

            var storedOtp = await _userManager.GetAuthenticationTokenAsync(user, "Havenly", "PasswordResetOTP");
            var storedExpiryStr = await _userManager.GetAuthenticationTokenAsync(user, "Havenly", "PasswordResetExpiry");

            if (string.IsNullOrEmpty(storedOtp) || storedOtp != otpCode.Trim())
            {
                return (false, null, "Invalid verification code. Please check and try again.");
            }

            if (DateTime.TryParse(storedExpiryStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expiryDate))
            {
                if (DateTime.UtcNow > expiryDate)
                {
                    return (false, null, "Verification code has expired. Please request a new code.");
                }
            }

            // Generate official ASP.NET Identity Password Reset Token
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Clean up OTP tokens
            await _userManager.RemoveAuthenticationTokenAsync(user, "Havenly", "PasswordResetOTP");
            await _userManager.RemoveAuthenticationTokenAsync(user, "Havenly", "PasswordResetExpiry");

            return (true, resetToken, null);
        }

        public async Task<(bool Succeeded, string? ErrorMessage)> ResetPasswordAsync(string email, string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
                return (false, "Invalid request. Please try again.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "Account not found.");

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                var error = string.Join(" ", result.Errors.Select(e => e.Description));
                return (false, error);
            }

            // Unlock lockout if account was locked
            await _userManager.SetLockoutEndDateAsync(user, null);

            return (true, null);
        }
    }
}
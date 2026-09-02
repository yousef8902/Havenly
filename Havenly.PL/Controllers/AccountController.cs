using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Havenly.BLL.ModelVMs.Account;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Havenly.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IBookingService _bookingService;
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IWebHostEnvironment _environment;

        public AccountController(
            IAccountService accountService,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IBookingService bookingService,
            IFavoriteRepository favoriteRepository,
            IReviewRepository reviewRepository,
            IWebHostEnvironment environment)
        {
            _accountService = accountService;
            _signInManager = signInManager;
            _userManager = userManager;
            _bookingService = bookingService;
            _favoriteRepository = favoriteRepository;
            _reviewRepository = reviewRepository;
            _environment = environment;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            bool isAjax = IsAjaxRequest();

            if (!ModelState.IsValid)
            {
                if (isAjax)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .Where(msg => !string.IsNullOrWhiteSpace(msg))
                        .ToList();
                    return Json(new { success = false, message = "Please correct the highlighted errors.", errors });
                }
                return View(model);
            }

            var result = await _accountService.RegisterAsync(model);

            if (result.Succeeded)
            {
                var redirectUrl = Url.Action("VerifyOtp", "Account", new { email = model.Email });
                if (isAjax)
                {
                    return Json(new { success = true, redirectUrl });
                }
                return RedirectToAction("VerifyOtp", new { email = model.Email });
            }

            var regErrors = result.Errors.Select(e => e.Description).ToList();
            if (isAjax)
            {
                return Json(new { success = false, message = "Registration failed.", errors = regErrors });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: /Account/VerifyOtp
        [HttpGet]
        public IActionResult VerifyOtp(string? email = null)
        {
            var model = new VerifyOtpVM { Email = email ?? string.Empty };
            return View(model);
        }

        // POST: /Account/VerifyOtp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(VerifyOtpVM model)
        {
            bool isAjax = IsAjaxRequest();

            if (!ModelState.IsValid)
            {
                if (isAjax)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .Where(msg => !string.IsNullOrWhiteSpace(msg))
                        .ToList();
                    return Json(new { success = false, message = "Please enter a valid 6-digit code.", errors });
                }
                return View(model);
            }

            var (succeeded, errorMessage) = await _accountService.VerifyOtpAsync(model.Email, model.OtpCode);

            if (succeeded)
            {
                var redirectUrl = Url.Action("Index", "Home");
                if (isAjax)
                {
                    return Json(new { success = true, redirectUrl });
                }
                return RedirectToAction("Index", "Home");
            }

            if (isAjax)
            {
                return Json(new { success = false, message = errorMessage ?? "Invalid code.", errors = new[] { errorMessage ?? "Invalid code." } });
            }

            ModelState.AddModelError(string.Empty, errorMessage ?? "Invalid verification code.");
            return View(model);
        }

        // POST: /Account/ResendOtp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendOtp(string email)
        {
            bool isAjax = IsAjaxRequest();

            var (succeeded, errorMessage) = await _accountService.ResendOtpAsync(email);

            if (succeeded)
            {
                if (isAjax)
                {
                    return Json(new { success = true, message = "A new 6-digit verification code has been sent to your email." });
                }
                TempData["SuccessMessage"] = "A new verification code has been sent.";
                return RedirectToAction("VerifyOtp", new { email });
            }

            if (isAjax)
            {
                return Json(new { success = false, message = errorMessage ?? "Failed to resend code." });
            }

            TempData["ErrorMessage"] = errorMessage ?? "Failed to resend code.";
            return RedirectToAction("VerifyOtp", new { email });
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            bool isAjax = IsAjaxRequest();

            if (!ModelState.IsValid)
            {
                if (isAjax)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .Where(msg => !string.IsNullOrWhiteSpace(msg))
                        .ToList();
                    return Json(new { success = false, message = "Please enter a valid email address.", errors });
                }
                return View(model);
            }

            var (succeeded, errorMessage) = await _accountService.SendPasswordResetOtpAsync(model.Email);
            if (succeeded)
            {
                var redirectUrl = Url.Action("VerifyResetOtp", "Account", new { email = model.Email });
                if (isAjax)
                {
                    return Json(new { success = true, redirectUrl });
                }
                return RedirectToAction("VerifyResetOtp", new { email = model.Email });
            }

            if (isAjax)
            {
                return Json(new { success = false, message = errorMessage ?? "Could not send reset code.", errors = new[] { errorMessage ?? "Could not send reset code." } });
            }

            ModelState.AddModelError(string.Empty, errorMessage ?? "Could not send reset code.");
            return View(model);
        }

        // GET: /Account/VerifyResetOtp
        [HttpGet]
        public IActionResult VerifyResetOtp(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToAction(nameof(ForgotPassword));
            }

            var model = new VerifyResetOtpVM { Email = email };
            return View(model);
        }

        // POST: /Account/VerifyResetOtp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyResetOtp(VerifyResetOtpVM model)
        {
            bool isAjax = IsAjaxRequest();

            if (!ModelState.IsValid)
            {
                if (isAjax)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .Where(msg => !string.IsNullOrWhiteSpace(msg))
                        .ToList();
                    return Json(new { success = false, message = "Please enter a valid 6-digit code.", errors });
                }
                return View(model);
            }

            var (succeeded, token, errorMessage) = await _accountService.VerifyPasswordResetOtpAsync(model.Email, model.OtpCode);
            if (succeeded && !string.IsNullOrEmpty(token))
            {
                var redirectUrl = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token });
                if (isAjax)
                {
                    return Json(new { success = true, redirectUrl });
                }
                return RedirectToAction("ResetPassword", new { email = model.Email, token = token });
            }

            if (isAjax)
            {
                return Json(new { success = false, message = errorMessage ?? "Invalid verification code.", errors = new[] { errorMessage ?? "Invalid verification code." } });
            }

            ModelState.AddModelError(string.Empty, errorMessage ?? "Invalid verification code.");
            return View(model);
        }

        // POST: /Account/ResendResetOtp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendResetOtp(string email)
        {
            bool isAjax = IsAjaxRequest();

            var (succeeded, errorMessage) = await _accountService.SendPasswordResetOtpAsync(email);
            if (succeeded)
            {
                if (isAjax)
                {
                    return Json(new { success = true, message = "A new 6-digit password reset code has been sent to your email." });
                }
                TempData["SuccessMessage"] = "A new reset code has been sent.";
                return RedirectToAction("VerifyResetOtp", new { email });
            }

            if (isAjax)
            {
                return Json(new { success = false, message = errorMessage ?? "Failed to resend reset code." });
            }

            TempData["ErrorMessage"] = errorMessage ?? "Failed to resend reset code.";
            return RedirectToAction("VerifyResetOtp", new { email });
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(nameof(ForgotPassword));
            }

            var model = new ResetPasswordVM { Email = email, Token = token };
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            bool isAjax = IsAjaxRequest();

            if (!ModelState.IsValid)
            {
                if (isAjax)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .Where(msg => !string.IsNullOrWhiteSpace(msg))
                        .ToList();
                    return Json(new { success = false, message = "Please check password requirements.", errors });
                }
                return View(model);
            }

            var (succeeded, errorMessage) = await _accountService.ResetPasswordAsync(model.Email, model.Token, model.Password);
            if (succeeded)
            {
                TempData["SuccessMessage"] = "Your password has been reset successfully! Please sign in with your new password.";
                var redirectUrl = Url.Action("Login", "Account");
                if (isAjax)
                {
                    return Json(new { success = true, redirectUrl });
                }
                return RedirectToAction("Login", "Account");
            }

            if (isAjax)
            {
                return Json(new { success = false, message = errorMessage ?? "Failed to reset password.", errors = new[] { errorMessage ?? "Failed to reset password." } });
            }

            ModelState.AddModelError(string.Empty, errorMessage ?? "Failed to reset password.");
            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            bool isAjax = IsAjaxRequest();

            if (!ModelState.IsValid)
            {
                if (isAjax)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .Where(msg => !string.IsNullOrWhiteSpace(msg))
                        .ToList();
                    return Json(new { success = false, message = "Please check the entered credentials.", errors });
                }
                return View(model);
            }

            var result = await _accountService.LoginAsync(model);

            if (result.Succeeded)
            {
                var redirectUrl = !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
                    ? returnUrl
                    : Url.Action("Index", "Home");

                if (isAjax)
                {
                    return Json(new { success = true, redirectUrl });
                }
                return RedirectToLocal(returnUrl);
            }

            if (result.IsLockedOut)
            {
                var lockMsg = "Your account is temporarily locked. Please try again later.";
                if (isAjax)
                {
                    return Json(new { success = false, message = lockMsg, errors = new[] { lockMsg } });
                }
                ModelState.AddModelError(string.Empty, lockMsg);
                return View(model);
            }

            if (result.IsNotAllowed)
            {
                var notAllowedMsg = "Please verify your email address to continue.";
                var verifyUrl = Url.Action("VerifyOtp", "Account", new { email = model.Email });
                if (isAjax)
                {
                    return Json(new { success = false, requiresVerification = true, redirectUrl = verifyUrl, message = notAllowedMsg, errors = new[] { notAllowedMsg } });
                }
                return RedirectToAction("VerifyOtp", new { email = model.Email });
            }

            // Generic error message: do not reveal whether email or password was wrong
            var genericError = "Invalid email or password.";
            if (isAjax)
            {
                return Json(new { success = false, message = genericError, errors = new[] { genericError } });
            }

            ModelState.AddModelError(string.Empty, genericError);
            return View(model);
        }

        private bool IsAjaxRequest()
        {
            return Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                   Request.Headers.Accept.ToString().Contains("application/json");
        }

        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your account is temporarily locked.");
                return View("Login");
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email?.Split('@')[0] ?? "Google User";

            if (!string.IsNullOrEmpty(email))
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new User
                    {
                        UserName = email,
                        Email = email,
                        Name = name,
                        Role = UserRoles.Guest,
                        Status = UserStatus.Active,
                        EmailConfirmed = true
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (createResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, UserRoles.Guest);
                        await _userManager.AddLoginAsync(user, info);
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                else
                {
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
            }

            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/Profile
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var model = await _accountService.GetUserProfileAsync(user.Id);
            if (model == null)
            {
                return RedirectToAction(nameof(Login));
            }

            return View(model);
        }

        // GET: /Account/EditProfile
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            var model = await _accountService.GetEditProfileAsync(user.Id);
            return View(model);
        }

        // POST: /Account/EditProfile
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? savedAvatarPath = null;
            if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
                Directory.CreateDirectory(uploadsFolder);

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var ext = Path.GetExtension(model.ProfilePictureFile.FileName).ToLowerInvariant();
                if (allowedExtensions.Contains(ext) && model.ProfilePictureFile.Length <= 5 * 1024 * 1024)
                {
                    var uniqueFileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfilePictureFile.CopyToAsync(stream);
                    }
                    savedAvatarPath = $"/uploads/avatars/{uniqueFileName}";
                }
            }

            var (success, error) = await _accountService.UpdateProfileAsync(user.Id, model, savedAvatarPath);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Unable to update profile.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Profile));
        }

        // GET: /Account/ViewProfile/{id}
        [HttpGet]
        public async Task<IActionResult> ViewProfile(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var model = await _accountService.GetPublicProfileAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        // GET: /Account/GetProfileSummary/{id} (JSON endpoint for quick modal preview)
        [HttpGet]
        public async Task<IActionResult> GetProfileSummary(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var model = await _accountService.GetPublicProfileAsync(id);
            if (model == null) return NotFound();

            return Json(new
            {
                userId = model.UserId,
                name = model.Name,
                email = model.Email,
                phoneNumber = model.PhoneNumber,
                profilePictureUrl = model.ProfilePictureUrl,
                bio = model.Bio,
                role = model.Role,
                status = model.Status,
                joinedDate = model.JoinedDate.ToString("MMMM yyyy"),
                totalProperties = model.TotalProperties,
                totalCompletedStays = model.TotalCompletedStays,
                totalReviews = model.TotalReviewsReceived,
                averageRating = model.AverageHostRating,
                properties = model.Properties.Take(3),
                guestStays = model.GuestStays.Take(3)
            });
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
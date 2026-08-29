using Havenly.BLL.ModelVMs.Account;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Havenly.BLL.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountService(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterVM model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "DuplicateEmail",
                        Description = "Email is already registered."
                    });
            }

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                UserName = model.Email,
                Status = UserStatus.Active
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return result;

            // Every newly registered user starts as Guest
            var roleResult = await _userManager.AddToRoleAsync(
                user,
                UserRoles.Guest);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return roleResult;
            }

            return IdentityResult.Success;
        }

        public async Task<SignInResult> LoginAsync(LoginVM model)
        {
            return await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);
        }

        public AuthenticationProperties ConfigureExternalLogin(string provider, string redirectUrl)
        {
            return _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        }


        public async Task<ExternalLoginResultVM> ExternalLoginCallbackAsync(string? returnUrl)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info is null)
            {
                return new ExternalLoginResultVM { Succeeded = false, ErrorMessage = "Error loading external login information." };
            }

            // Try signing in with this external provider if already linked to an account
            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return new ExternalLoginResultVM { Succeeded = true };
            }

            // Not linked yet — check if a user with this email already exists
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return new ExternalLoginResultVM { Succeeded = false, ErrorMessage = "Google account has no email." };
            }

            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser is not null)
            {
                // Link this Google login to the existing account
                var linkResult = await _userManager.AddLoginAsync(existingUser, info);
                if (!linkResult.Succeeded)
                {
                    return new ExternalLoginResultVM { Succeeded = false, ErrorMessage = "Could not link Google account." };
                }
                await _signInManager.SignInAsync(existingUser, isPersistent: false);
                return new ExternalLoginResultVM { Succeeded = true };
            }

            // Brand new user signing up via Google
            var name = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email;
            var newUser = new User
            {
                UserName = email,
                Email = email,
                Name = name,
                Status = UserStatus.Active,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(newUser);
            if (!createResult.Succeeded)
            {
                return new ExternalLoginResultVM { Succeeded = false, ErrorMessage = "Could not create account." };
            }

            await _userManager.AddToRoleAsync(newUser, UserRoles.Guest);
            await _userManager.AddLoginAsync(newUser, info);
            await _signInManager.SignInAsync(newUser, isPersistent: false);

            return new ExternalLoginResultVM { Succeeded = true };
        }
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
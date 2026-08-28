using Havenly.BLL.ModelVM.Account;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Microsoft.AspNetCore.Identity;

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

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
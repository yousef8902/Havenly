
using Havenly.BLL.ModelVMs.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterVM model);

        Task<SignInResult> LoginAsync(LoginVM model);

        AuthenticationProperties ConfigureExternalLogin(string provider, string redirectUrl);
        Task<ExternalLoginResultVM> ExternalLoginCallbackAsync(string? returnUrl);

        Task LogoutAsync();
    }
}


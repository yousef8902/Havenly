
<<<<<<< HEAD
using Havenly.BLL.ModelVMs.Account;
using Microsoft.AspNetCore.Authentication;
=======
using Havenly.BLL.ModelVM.Account;
>>>>>>> 63b1b37 (add search by review and change relation between (review->booking) to (review->user))
using Microsoft.AspNetCore.Identity;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterVM model);

        Task<SignInResult> LoginAsync(LoginVM model);

<<<<<<< HEAD
        AuthenticationProperties ConfigureExternalLogin(string provider, string redirectUrl);
        Task<ExternalLoginResultVM> ExternalLoginCallbackAsync(string? returnUrl);

=======
>>>>>>> 63b1b37 (add search by review and change relation between (review->booking) to (review->user))
        Task LogoutAsync();
    }
}


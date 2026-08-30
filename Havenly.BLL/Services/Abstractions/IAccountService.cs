
using Havenly.BLL.ModelVM.Account;
using Microsoft.AspNetCore.Identity;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterVM model);

        Task<SignInResult> LoginAsync(LoginVM model);

        Task LogoutAsync();
    }
}


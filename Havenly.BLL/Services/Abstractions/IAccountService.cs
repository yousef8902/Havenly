using System.Threading.Tasks;
using Havenly.BLL.ModelVMs.Account;
using Microsoft.AspNetCore.Identity;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterVM model);

        Task<SignInResult> LoginAsync(LoginVM model);

        Task<(bool Succeeded, string? ErrorMessage)> VerifyOtpAsync(string email, string otpCode);

        Task<(bool Succeeded, string? ErrorMessage)> ResendOtpAsync(string email);

        Task LogoutAsync();

        Task<UserProfileVM?> GetUserProfileAsync(string userId);

        Task<EditProfileVM?> GetEditProfileAsync(string userId);

        Task<(bool Succeeded, string? ErrorMessage)> UpdateProfileAsync(string userId, EditProfileVM model, string? savedAvatarPath = null);

        Task<PublicUserProfileVM?> GetPublicProfileAsync(string userId);

        Task<(bool Succeeded, string? ErrorMessage)> SendPasswordResetOtpAsync(string email);

        Task<(bool Succeeded, string? Token, string? ErrorMessage)> VerifyPasswordResetOtpAsync(string email, string otpCode);

        Task<(bool Succeeded, string? ErrorMessage)> ResetPasswordAsync(string email, string token, string newPassword);
    }
}

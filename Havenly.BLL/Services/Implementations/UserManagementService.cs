using Havenly.BLL.ModelVMs;
using Havenly.BLL.ModelVMs.Admin;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.AspNetCore.Identity;
namespace Havenly.BLL.Services.Implementations
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<User> userManager;
        private readonly IBookingRepository bookingRepository;

        public UserManagementService(UserManager<User> userManager, IBookingRepository bookingRepository)
        {
            this.userManager = userManager;
            this.bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<MemberVM>> GetAllMembers()
        {
            var users = userManager.Users.Where(u => u.Status != UserStatus.Deleted).ToList();
            var result = new List<MemberVM>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                var bookingCount = (await bookingRepository.Find(b => b.GuestUserID == user.Id)).Count();
                var isLockedOut = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow;

                result.Add(new MemberVM
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "Unknown",
                    Status = isLockedOut ? "Suspended" : "Active",
                    BookingsCount = bookingCount
                });
            }

            return result;
        }

        public async Task<bool> SuspendUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return false;

            user.LockoutEnabled = true;
            var result = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            if (!result.Succeeded) return false;

            user.Status = UserStatus.Suspended;
            await userManager.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ReinstateUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return false;

            var result = await userManager.SetLockoutEndDateAsync(user, null);
            if (!result.Succeeded) return false;

            user.Status = UserStatus.Active;
            await userManager.UpdateAsync(user);
            return true;
        }
    }
}
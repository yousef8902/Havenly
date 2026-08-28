using Havenly.BLL.ModelVMs;
using Havenly.BLL.ModelVMs.Admin;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Havenly.BLL.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IUserRepository userRepository;
        private readonly IListingRepository listingRepository;
        private readonly IBookingRepository bookingRepository;

        public ReportService(
            IUserRepository userRepository,
            IListingRepository listingRepository,
            IBookingRepository bookingRepository)
        {
            this.userRepository = userRepository;
            this.listingRepository = listingRepository;
            this.bookingRepository = bookingRepository;
        }

        public async Task<PlatformStatsVM> GetPlatformStats()
        {
            var users = await userRepository.Find(u => u.Status ==UserStatus.Active);
            var listings = await listingRepository.Find(l => l.ListingStatus != ListingStatus.Declined);
            var pendingListings = await listingRepository.GetByStatus(ListingStatus.Pending);
            var nonCancelledBookings = await bookingRepository.Find(b => b.Status != BookingStatus.Cancelled);

            return new PlatformStatsVM
            {
                MemberCount = users.Count(),
                ListingCount = listings.Count(),
                PendingReviewCount = pendingListings.Count(),
                GrossBookings = nonCancelledBookings.Sum(b => b.TotalPrice)
            };
        }
    }
}
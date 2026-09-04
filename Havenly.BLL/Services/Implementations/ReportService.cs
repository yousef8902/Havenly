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
            };//need to add average ratings


        }

        public async Task<MonthlyStatsVM> GetMonthlyStats()
        {
            var users = await userRepository.Find(
                u => u.Status == UserStatus.Active);

            var bookings = await bookingRepository.Find(
                b => b.Status != BookingStatus.Cancelled);

            var today = DateTime.UtcNow;

            var months = Enumerable.Range(0, 12)
                .Select(i => new DateTime(
                    today.Year,
                    today.Month,
                    1
                ).AddMonths(-11 + i))
                .ToList();

            var result = new MonthlyStatsVM();

            foreach (var month in months)
            {
                var nextMonth = month.AddMonths(1);

                result.Months.Add(month.ToString("MMM"));
                result.Years.Add(month.Year);
                result.MonthNumbers.Add(month.Month);
                result.FullMonthNames.Add(month.ToString("MMMM yyyy"));

                result.Signups.Add(
                    users.Count(u =>
                        u.JoinedDate >= month &&
                        u.JoinedDate < nextMonth)
                );

                result.Bookings.Add(
                    bookings.Count(b =>
                        b.CreatedDate >= month &&
                        b.CreatedDate < nextMonth)
                );
            }

            return result;
        }
    }
}
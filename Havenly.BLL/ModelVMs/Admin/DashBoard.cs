using Havenly.BLL.ModelVMs.Admin;
using Havenly.DAL.Entities;

namespace Havenly.BLL.ModelVMs
{
    public class AdminDashboardVM
    {
        public PlatformStatsVM Stats { get; set; }

        public IEnumerable<PendingListing> PendingListings { get; set; }
            = Enumerable.Empty<PendingListing>();

        public AdminMembersVM? MembersResult { get; set; }

        public MonthlyStatsVM MonthlyStats { get; set; }
            = new MonthlyStatsVM();
    }
}
using Havenly.BLL.ModelVMs.Admin;
using Havenly.DAL.Entities;

namespace Havenly.BLL.ModelVMs
{
    public class AdminDashboardVM
    {
        public PlatformStatsVM Stats { get; set; }
        public IEnumerable<Listing> PendingListings { get; set; }
        public IEnumerable<MemberVM> Members { get; set; }
    }
}
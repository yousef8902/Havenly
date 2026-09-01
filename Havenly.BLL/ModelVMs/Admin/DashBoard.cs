using Havenly.BLL.ModelVMs.Admin;
using Havenly.DAL.Entities;

namespace Havenly.BLL.ModelVMs
{
    public class AdminDashboardVM
    {
        public PlatformStatsVM Stats { get; set; }
        public IEnumerable<PendingListing> PendingListings { get; set; }

        public AdminMembersVM? MembersResult { get; set; }//mariam
    }
}
using Havenly.BLL.ModelVMs.Admin;
using Havenly.DAL.Entities;
using System.Collections.Generic;

namespace Havenly.BLL.ModelVMs
{
    public class AdminDashboardVM
    {
        public PlatformStatsVM Stats { get; set; }
        public IEnumerable<Listing> PendingListings { get; set; }
    }
}
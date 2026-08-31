namespace Havenly.BLL.ModelVMs.Admin
{
    public class PlatformStatsVM
    {
        public int MemberCount { get; set; }
        public int ListingCount { get; set; }
        public decimal GrossBookings { get; set; }
        public int PendingReviewCount { get; set; }
        public int TotalBookings { get; set; }//mariam
    }
}

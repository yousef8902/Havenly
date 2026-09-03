namespace Havenly.BLL.ModelVMs.Admin
{
    public class MonthlyStatsVM
    {
        public List<string> Months { get; set; } = new();

        public List<int> Signups { get; set; } = new();

        public List<int> Bookings { get; set; } = new();
    }
}
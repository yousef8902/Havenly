namespace Havenly.BLL.ModelVMs.Admin
{
    public class MonthlyStatsVM
    {
        public List<string> Months { get; set; } = new();

        public List<int> Years { get; set; } = new();

        public List<int> MonthNumbers { get; set; } = new();

        public List<string> FullMonthNames { get; set; } = new();

        public List<int> Signups { get; set; } = new();

        public List<int> Bookings { get; set; } = new();
    }
}
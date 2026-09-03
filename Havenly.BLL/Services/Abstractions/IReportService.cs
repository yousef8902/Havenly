using Havenly.BLL.ModelVMs.Admin;


namespace Havenly.BLL.Services.Abstractions
{
    public interface IReportService
    {
        Task<PlatformStatsVM> GetPlatformStats();
        Task<MonthlyStatsVM> GetMonthlyStats();
    }
}

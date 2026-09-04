using System.Threading.Tasks;
using Havenly.BLL.ModelVMs.Host;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IHostCalendarService
    {
        Task<HostCalendarPageVM> GetCalendarDataAsync(string hostUserId, long? propertyId, int? year, int? month);
        Task<bool> BlockDatesAsync(string hostUserId, BlockDateRequestVM model);
        Task<bool> UnblockDatesAsync(string hostUserId, long blockedDateId);
        Task<PropertyAvailabilityDatesVM> GetPropertyDisabledDatesAsync(long propertyId);
    }
}

using Havenly.BLL.ModelVMs;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IPropertyService
    {
        Task<IEnumerable<PropertyCardVM>> GetPropertiesAsync(string? city, string? category, int guests = 2);
        Task<PropertyDetailsVM?> GetPropertyDetailsByIdAsync(long id);
    }
}
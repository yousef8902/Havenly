using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.EntityFrameworkCore;
using Havenly.BLL.ModelVMs;
using AutoMapper;

namespace Havenly.BLL.Services.Implementations
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public PropertyService(IPropertyRepository propertyRepository, IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PropertyCardVM>> GetPropertiesAsync(string? city, string? category, int guests = 2)
        {
            var properties = await _propertyRepository.GetAll();
            var query = properties.AsQueryable();

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(p => p.Address.City != null && p.Address.City.Contains(city, StringComparison.OrdinalIgnoreCase));
            }

            if (guests > 0)
            {
                query = query.Where(p => p.NumberOfGuests >= guests);
            }

          
            return _mapper.Map<IEnumerable<PropertyCardVM>>(query);
        }

        public async Task<PropertyDetailsVM?> GetPropertyDetailsByIdAsync(long id)
        {
            var property = await _propertyRepository.GetDetailbyId(id);

            if (property == null)
            {
                return null;
            }

            return _mapper.Map<PropertyDetailsVM>(property);
        }
    }
}
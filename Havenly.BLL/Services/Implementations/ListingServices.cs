using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;


namespace Havenly.BLL.Services.Implementations
{
    public class ListingServices:IListingServices
    {
        private readonly IListingRepository listingRepository;
        public ListingServices(IListingRepository listingRepository) {
            this.listingRepository = listingRepository;
        
        }

        public async Task<bool> ApproveListing(long id)
        {
           return await listingRepository.ApproveListing(id);
        }

        public async Task<bool> DeclineListing(long id)
        {
           return await listingRepository.DeclineListing(id);
        }

        public async Task<IEnumerable<Listing>> GetPendingListings()
        {
            return await listingRepository.GetByStatus(ListingStatus.Pending);
        }
    }
}

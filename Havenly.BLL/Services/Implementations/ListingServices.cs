using AutoMapper;
using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.EntityFrameworkCore;


namespace Havenly.BLL.Services.Implementations
{
    public class ListingServices:IListingServices
    {
        private readonly IListingRepository listingRepository;
        private readonly IMapper _mapper;
        public ListingServices(IListingRepository listingRepository, IMapper mapper) {
            this.listingRepository = listingRepository;
            this._mapper = mapper;
        }

        public async Task<bool> ApproveListing(long id)
        {
           return await listingRepository.ApproveListing(id);
        }

        public async Task<bool> DeclineListing(long id)
        {
           return await listingRepository.DeclineListing(id);
        }

        public async Task<IEnumerable<PendingListing>> GetPendingListings()
        {
           var listings = await listingRepository.GetByStatus(ListingStatus.Pending);
           

            // AutoMapper maps entities to ViewModels
            return _mapper.Map<IEnumerable<PendingListing>>(listings);
        }

        public async Task<IEnumerable<Listing>> GetListingsAsync(ListingStatus status)
        {
            return await  listingRepository.GetByStatus(status);
        }
    }
}

using Havenly.BLL.ModelVMs;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using System.Reflection;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IListingServices
    {

        Task <bool>ApproveListing(long id);
        Task<bool> DeclineListing(long id);
        Task<IEnumerable<PendingListing>> GetPendingListings();//maraim

        Task<IEnumerable<Listing>> GetListingsAsync(ListingStatus s  );//mariam
    }
}

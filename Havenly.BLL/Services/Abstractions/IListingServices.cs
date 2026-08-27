using Havenly.DAL.Entities;
using System.Reflection;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IListingServices
    {

        Task <bool>ApproveListing(long id);
        Task<bool> DeclineListing(long id);
        Task<IEnumerable<Listing>> GetPendingListings();
    }
}

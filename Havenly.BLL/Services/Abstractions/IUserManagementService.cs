using Havenly.BLL.ModelVMs;
using Havenly.BLL.ModelVMs.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IUserManagementService
    {
        Task<IEnumerable<MemberVM>> GetAllMembers();
        Task<bool> SuspendUser(string userId);
        Task<bool> ReinstateUser(string userId);
        Task<bool> ApproveHost(string userId);
        Task<bool> RejectHost(string userId);
    }
}
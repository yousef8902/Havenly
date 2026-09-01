using System.Collections.Generic;
using System.Threading.Tasks;
using Havenly.DAL.Entities;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IPaymobService
    {
        Task<string?> CreatePaymentTokenAsync(Booking booking, string guestEmail, string guestName, string guestPhone);
        string GetIframeUrl(string paymentToken);
        bool ValidateHmac(IDictionary<string, string> queryParams);
    }
}

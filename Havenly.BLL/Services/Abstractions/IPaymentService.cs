using System.Threading.Tasks;
using Havenly.BLL.ModelVMs.Payment;
using Havenly.DAL.Entities;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IPaymentService
    {
        Task<Payment> CreateOrGetPendingPaymentAsync(long bookingId);
        Task<bool> CompletePaymentAsync(long paymentId, string transactionId, string gateway = "Paymob");
        Task<bool> FailPaymentAsync(long paymentId, string? reason = null);
        Task<bool> ProcessHostPayoutAsync(long paymentId);
        Task<AdminPaymentHistoryVM> GetPaymentHistoryAsync();
        Task<HostPayoutsVM> GetHostPayoutsAsync(string hostUserId);
        Task<CheckoutVM?> GetCheckoutDetailsAsync(long bookingId);
        Task<PaymentSuccessVM?> GetPaymentReceiptAsync(long paymentId);
    }
}

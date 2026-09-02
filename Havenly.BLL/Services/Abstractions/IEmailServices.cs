using System;
using System.Threading.Tasks;
using Havenly.DAL.Entities;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IEmailServices
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);

        // Account Verification & Password Reset
        Task SendEmailVerificationOtpAsync(string toEmail, string userName, string otpCode);
        Task SendPasswordResetOtpAsync(string toEmail, string userName, string otpCode);

        // Scenario 1: Guest creates booking -> Notify Host
        Task SendBookingRequestToHostAsync(
            string hostEmail,
            string hostName,
            string guestName,
            string propertyName,
            DateTime checkIn,
            DateTime checkOut,
            decimal totalPrice,
            int totalNights,
            long bookingId);

        // Scenario 2: Host approves/declines booking -> Notify Guest
        Task SendBookingStatusToGuestAsync(
            string guestEmail,
            string guestName,
            string hostName,
            string propertyName,
            DateTime checkIn,
            DateTime checkOut,
            decimal totalPrice,
            string city,
            string country,
            bool isApproved,
            long bookingId);

        // Scenario 3: Host creates/submits property -> Notify Admin
        Task SendListingSubmittedToAdminAsync(
            string adminEmail,
            string hostName,
            string hostEmail,
            string propertyName,
            string category,
            string location,
            decimal price,
            long propertyId);

        // Scenario 4: Admin approves/declines listing -> Notify Host
        Task SendListingDecisionToHostAsync(
            string hostEmail,
            string hostName,
            string propertyName,
            bool isApproved,
            long propertyId);

        // Scenario 5: Admin approves/declines host registration application
        Task SendHostApplicationDecisionAsync(
            string hostEmail,
            string hostName,
            bool isApproved,
            string? notes = null);

        // Legacy helper overloads
        Task SendEmailRequestbookingToHost(string toEmail, Booking booking);
        Task ReciveEmailRequestbookingFromHost(string toEmail, Booking booking);
        Task SendEmailRequestPropertyToAdmin(string toEmail, Booking booking);
        Task ReciveEmailRequestPropertyFromAdmin(string toEmail, Booking booking);
    }
}

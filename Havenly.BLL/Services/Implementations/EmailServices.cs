using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Havenly.BLL.Services.Implementations
{
    public class EmailServices : IEmailServices
    {
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _fromEmail;
        private readonly string _password;
        private readonly string _displayName;
        private readonly string _baseUrl;
        private readonly ILogger<EmailServices>? _logger;

        public EmailServices(IConfiguration configuration, ILogger<EmailServices>? logger = null)
        {
            _logger = logger;
            _smtpServer = configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            _port = int.TryParse(configuration["EmailSettings:Port"], out var port) ? port : 587;
            _fromEmail = (configuration["EmailSettings:SenderEmail"] ?? "").Trim();
            _password = (configuration["EmailSettings:SenderPassword"] ?? "").Replace(" ", "").Trim();
            _displayName = configuration["EmailSettings:SenderDisplayName"] ?? "Havenly";
            _baseUrl = (configuration["App:BaseUrl"] ?? configuration["ApplicationUrl"] ?? "https://localhost:7145").TrimEnd('/');
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                return;

            // Simulator mode for local dev when sender credentials are missing
            if (string.IsNullOrWhiteSpace(_fromEmail) || string.IsNullOrWhiteSpace(_password))
            {
                var simulatedMsg = $"[EMAIL SIMULATOR]\nTo: {toEmail}\nSubject: {subject}\nBody: {body}\n";
                Console.WriteLine(simulatedMsg);
                _logger?.LogInformation("{Message}", simulatedMsg);
                return;
            }

            try
            {
                using var client = new SmtpClient(_smtpServer, _port)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_fromEmail, _password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _displayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] Failed to send email to {toEmail}: {ex.Message}");
                _logger?.LogError(ex, "Failed to send email to {Email}", toEmail);
            }
        }

        // Account Verification OTP
        public async Task SendEmailVerificationOtpAsync(string toEmail, string userName, string otpCode)
        {
            string subject = $"{otpCode} is your Havenly verification code";
            string displayName = string.IsNullOrWhiteSpace(userName) ? "there" : userName;

            string contentHtml = $@"
                <p style=""font-size: 15px; color: #334155; line-height: 1.6; margin-bottom: 24px;"">
                    Welcome to Havenly! Please use the 6-digit verification code below to confirm your email and complete your registration:
                </p>
                <div style=""background-color: #f0fdf4; border: 2px dashed #059669; border-radius: 14px; padding: 20px; text-align: center; margin: 24px 0;"">
                    <div style=""font-size: 36px; font-weight: 800; letter-spacing: 8px; color: #065f46; font-family: monospace;"">{otpCode}</div>
                </div>
                <p style=""font-size: 13px; color: #64748b; line-height: 1.5;"">
                    ⏱️ This verification code will expire in <strong>15 minutes</strong>. If you did not request this, please disregard this email.
                </p>";

            string html = BuildHtmlWrapper("Confirm your email address", displayName, contentHtml, null, null);
            await SendEmailAsync(toEmail, subject, html, isHtml: true);
        }

        // Password Reset OTP
        public async Task SendPasswordResetOtpAsync(string toEmail, string userName, string otpCode)
        {
            string subject = $"{otpCode} is your Havenly password reset code";
            string displayName = string.IsNullOrWhiteSpace(userName) ? "there" : userName;

            string contentHtml = $@"
                <p style=""font-size: 15px; color: #334155; line-height: 1.6; margin-bottom: 24px;"">
                    We received a request to reset your Havenly account password. Please use the 6-digit verification code below to proceed:
                </p>
                <div style=""background-color: #fefce8; border: 2px dashed #ca8a04; border-radius: 14px; padding: 20px; text-align: center; margin: 24px 0;"">
                    <div style=""font-size: 36px; font-weight: 800; letter-spacing: 8px; color: #854d0e; font-family: monospace;"">{otpCode}</div>
                </div>
                <p style=""font-size: 13px; color: #64748b; line-height: 1.5;"">
                    ⏱️ This code will expire in <strong>15 minutes</strong>. If you did not request a password reset, you can safely ignore this email and your password will remain unchanged.
                </p>";

            string html = BuildHtmlWrapper("Reset your password", displayName, contentHtml, null, null);
            await SendEmailAsync(toEmail, subject, html, isHtml: true);
        }

        // Scenario 1: Guest creates booking -> Notify Host
        public async Task SendBookingRequestToHostAsync(
            string hostEmail,
            string hostName,
            string guestName,
            string propertyName,
            DateTime checkIn,
            DateTime checkOut,
            decimal totalPrice,
            int totalNights,
            long bookingId)
        {
            string subject = $"New Booking Request: {guestName} booked \"{propertyName}\" (#{bookingId})";
            string displayName = string.IsNullOrWhiteSpace(hostName) ? "Host" : hostName;

            string contentHtml = $@"
                <p style=""font-size: 15px; color: #334155; line-height: 1.6; margin-bottom: 20px;"">
                    Great news! Guest <strong>{guestName}</strong> has submitted a booking request for your property <strong>{propertyName}</strong>.
                </p>
                <div style=""background-color: #f8fafc; border: 1px solid #e2e8f0; border-radius: 14px; padding: 20px; margin: 20px 0;"">
                    <table style=""width: 100%; border-collapse: collapse; font-size: 14px;"">
                        <tr>
                            <td style=""padding: 6px 0; color: #64748b; font-weight: 600;"">Guest Name:</td>
                            <td style=""padding: 6px 0; color: #0f172a; font-weight: 600; text-align: right;"">{guestName}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 6px 0; color: #64748b;"">Property:</td>
                            <td style=""padding: 6px 0; color: #0f172a; text-align: right;"">{propertyName}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 6px 0; color: #64748b;"">Check-in:</td>
                            <td style=""padding: 6px 0; color: #0f172a; text-align: right;"">{checkIn:MMM dd, yyyy}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 6px 0; color: #64748b;"">Check-out:</td>
                            <td style=""padding: 6px 0; color: #0f172a; text-align: right;"">{checkOut:MMM dd, yyyy}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 6px 0; color: #64748b;"">Duration:</td>
                            <td style=""padding: 6px 0; color: #0f172a; text-align: right;"">{totalNights} {(totalNights == 1 ? "night" : "nights")}</td>
                        </tr>
                        <tr style=""border-top: 1px solid #e2e8f0;"">
                            <td style=""padding: 10px 0 0 0; color: #065f46; font-weight: 700; font-size: 15px;"">Total Payout:</td>
                            <td style=""padding: 10px 0 0 0; color: #065f46; font-weight: 800; font-size: 16px; text-align: right;"">{totalPrice:N2} EGP</td>
                        </tr>
                    </table>
                </div>
                <p style=""font-size: 14px; color: #475569; margin-bottom: 24px;"">
                    Please log in to your Host Dashboard to accept or reject this booking request.
                </p>";

            string html = BuildHtmlWrapper("New Booking Request", displayName, contentHtml, "Review in Host Dashboard", "/Host/Bookings");
            await SendEmailAsync(hostEmail, subject, html, isHtml: true);
        }

        // Scenario 2: Host approves/declines booking -> Notify Guest
        public async Task SendBookingStatusToGuestAsync(
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
            long bookingId)
        {
            string displayName = string.IsNullOrWhiteSpace(guestName) ? "Guest" : guestName;
            string hostDisplayName = string.IsNullOrWhiteSpace(hostName) ? "The host" : hostName;
            string locationStr = string.IsNullOrWhiteSpace(city) ? country : $"{city}, {country}";

            string subject;
            string title;
            string contentHtml;
            string ctaText;
            string ctaUrl;

            if (isApproved)
            {
                subject = $"Booking Approved! Your stay at \"{propertyName}\" is confirmed";
                title = "🎉 Booking Confirmed!";
                contentHtml = $@"
                    <p style=""font-size: 15px; color: #334155; line-height: 1.6; margin-bottom: 20px;"">
                        Great news! Host <strong>{hostDisplayName}</strong> has <strong>approved</strong> your booking request for <strong>{propertyName}</strong>.
                    </p>
                    <div style=""background-color: #f0fdf4; border: 1px solid #bbf7d0; border-radius: 14px; padding: 20px; margin: 20px 0;"">
                        <table style=""width: 100%; border-collapse: collapse; font-size: 14px;"">
                            <tr>
                                <td style=""padding: 6px 0; color: #166534; font-weight: 600;"">Property:</td>
                                <td style=""padding: 6px 0; color: #0f172a; font-weight: 600; text-align: right;"">{propertyName}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 6px 0; color: #166534;"">Location:</td>
                                <td style=""padding: 6px 0; color: #0f172a; text-align: right;"">{locationStr}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 6px 0; color: #166534;"">Check-in:</td>
                                <td style=""padding: 6px 0; color: #0f172a; text-align: right;"">{checkIn:MMM dd, yyyy}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 6px 0; color: #166534;"">Check-out:</td>
                                <td style=""padding: 6px 0; color: #0f172a; text-align: right;"">{checkOut:MMM dd, yyyy}</td>
                            </tr>
                            <tr style=""border-top: 1px solid #bbf7d0;"">
                                <td style=""padding: 10px 0 0 0; color: #166534; font-weight: 700; font-size: 15px;"">Total Amount:</td>
                                <td style=""padding: 10px 0 0 0; color: #166534; font-weight: 800; font-size: 16px; text-align: right;"">{totalPrice:N2} EGP</td>
                            </tr>
                        </table>
                    </div>
                    <p style=""font-size: 14px; color: #475569; margin-bottom: 24px;"">
                        Your itinerary and reservation details are ready in your account. Pack your bags and enjoy your stay!
                    </p>";
                ctaText = "View My Bookings";
                ctaUrl = "/Booking/MyBookings";
            }
            else
            {
                subject = $"Update on your booking request for \"{propertyName}\"";
                title = "Booking Request Update";
                contentHtml = $@"
                    <p style=""font-size: 15px; color: #334155; line-height: 1.6; margin-bottom: 20px;"">
                        Host <strong>{hostDisplayName}</strong> was unable to accept your booking request for <strong>{propertyName}</strong> ({checkIn:MMM dd} – {checkOut:MMM dd}).
                    </p>
                    <div style=""background-color: #fef2f2; border: 1px solid #fecaca; border-radius: 14px; padding: 18px; margin: 20px 0; font-size: 14px; color: #991b1b;"">
                        ℹ️ No charges have been applied, or any authorization hold has been released back to your payment method.
                    </div>
                    <p style=""font-size: 14px; color: #475569; margin-bottom: 24px;"">
                        Don't worry—there are many other wonderful homes and villas waiting for you on Havenly!
                    </p>";
                ctaText = "Explore Other Stays";
                ctaUrl = "/Property";
            }

            string html = BuildHtmlWrapper(title, displayName, contentHtml, ctaText, ctaUrl);
            await SendEmailAsync(guestEmail, subject, html, isHtml: true);
        }

        // Scenario 3: Host creates/submits property -> Notify Admin
        public async Task SendListingSubmittedToAdminAsync(
            string adminEmail,
            string hostName,
            string hostEmail,
            string propertyName,
            string category,
            string location,
            decimal price,
            long propertyId)
        {
            string subject = $"[Admin Review] New Listing Submitted: \"{propertyName}\" by {hostName}";
            string displayName = "Admin Team";

            string contentHtml = $@"
                <p style=""font-size: 15px; color: #334155; line-height: 1.6; margin-bottom: 20px;"">
                    Host <strong>{hostName}</strong> ({hostEmail}) has submitted a new property listing for administrative review and publication approval.
                </p>
                <div style=""background-color: #f8fafc; border: 1px solid #e2e8f0; border-radius: 14px; padding: 20px; margin: 20px 0;"">
                    <table style=""width: 100%; border-collapse: collapse; font-size: 14px;"">
                        <tr>
                            <td style=""padding: 6px 0; color: #64748b; font-weight: 600;"">Property Name:</td>
                            <td style=""padding: 6px 0; color: #0f172a; font-weight: 600; text-align: right;"">{propertyName}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 6px 0; color: #64748b;"">Host:</td>
                            <td style=""padding: 6px 0; color: #0f172a; text-align: right;"">{hostName}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 6px 0; color: #64748b;"">Category:</td>
                            <td style=""padding: 6px 0; color: #0f172a; text-align: right;"">{category}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 6px 0; color: #64748b;"">Location:</td>
                            <td style=""padding: 6px 0; color: #0f172a; text-align: right;"">{location}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 6px 0; color: #64748b;"">Price / Night:</td>
                            <td style=""padding: 6px 0; color: #065f46; font-weight: 700; text-align: right;"">{price:N2} EGP</td>
                        </tr>
                    </table>
                </div>
                <p style=""font-size: 14px; color: #475569; margin-bottom: 24px;"">
                    Please review this property's photos, details, and description in the Admin Console to approve or decline publication.
                </p>";

            string html = BuildHtmlWrapper("New Property Submission", displayName, contentHtml, "Open Admin Console", "/Admin/DashBoard");
            await SendEmailAsync(adminEmail, subject, html, isHtml: true);
        }

        // Scenario 4: Admin approves/declines listing -> Notify Host
        public async Task SendListingDecisionToHostAsync(
            string hostEmail,
            string hostName,
            string propertyName,
            bool isApproved,
            long propertyId)
        {
            string displayName = string.IsNullOrWhiteSpace(hostName) ? "Host" : hostName;
            string subject;
            string title;
            string contentHtml;
            string ctaText;
            string ctaUrl;

            if (isApproved)
            {
                subject = $"Congratulations! Your listing \"{propertyName}\" is now LIVE on Havenly";
                title = "🎉 Listing Approved & Published!";
                contentHtml = $@"
                    <p style=""font-size: 15px; color: #334155; line-height: 1.6; margin-bottom: 20px;"">
                        Congratulations! Your property listing <strong>{propertyName}</strong> has been reviewed and <strong>approved</strong> by our administration team.
                    </p>
                    <div style=""background-color: #f0fdf4; border: 1px solid #bbf7d0; border-radius: 14px; padding: 20px; margin: 20px 0; font-size: 14px; color: #166534;"">
                        ✅ Your listing is now publicly visible on the Havenly Explore page. Guests worldwide can discover, save, and book your stay.
                    </div>
                    <p style=""font-size: 14px; color: #475569; margin-bottom: 24px;"">
                        You can manage your listing, update photos, or view incoming reservations anytime in your Host Dashboard.
                    </p>";
                ctaText = "Manage My Properties";
                ctaUrl = "/Host/ListProperties";
            }
            else
            {
                subject = $"Update on your listing submission: \"{propertyName}\"";
                title = "Listing Review Update";
                contentHtml = $@"
                    <p style=""font-size: 15px; color: #334155; line-height: 1.6; margin-bottom: 20px;"">
                        Our administration team has reviewed your submission for <strong>{propertyName}</strong> and was unable to approve it for publication at this time.
                    </p>
                    <div style=""background-color: #f8fafc; border: 1px solid #e2e8f0; border-radius: 14px; padding: 18px; margin: 20px 0; font-size: 14px; color: #475569;"">
                        💡 Please make sure your listing includes high-quality photos, clear room & amenity descriptions, and accurate location details before resubmitting.
                    </div>
                    <p style=""font-size: 14px; color: #475569; margin-bottom: 24px;"">
                        You can edit your property details and resubmit it for review in your Host Dashboard.
                    </p>";
                ctaText = "Edit Property Listing";
                ctaUrl = "/Host/ListProperties";
            }

            string html = BuildHtmlWrapper(title, displayName, contentHtml, ctaText, ctaUrl);
            await SendEmailAsync(hostEmail, subject, html, isHtml: true);
        }

        public async Task SendHostApplicationDecisionAsync(string hostEmail, string hostName, bool isApproved, string? notes = null)
        {
            string displayName = string.IsNullOrWhiteSpace(hostName) ? "Host Partner" : hostName;
            string subject;
            string title;
            string contentHtml;
            string ctaText;
            string ctaUrl;

            if (isApproved)
            {
                subject = "🎉 Your Havenly Host Account is Approved!";
                title = "Welcome to Havenly Hosting";
                contentHtml = $@"
                    <p style=""font-size: 15px; color: #334155; line-height: 1.6; margin-bottom: 20px;"">
                        Congratulations! Your host verification document has been reviewed and <strong style=""color: #059669;"">approved</strong> by our administration team.
                    </p>
                    <div style=""background-color: #f0fdf4; border: 1px solid #bbf7d0; border-radius: 14px; padding: 18px; margin: 20px 0;"">
                        <div style=""font-size: 14px; font-weight: 700; color: #166534; margin-bottom: 6px;"">✅ Verified Egyptian Host Status Active</div>
                        <div style=""font-size: 13px; color: #15803d; line-height: 1.5;"">
                            You now have complete access to the Havenly Host Workspace. You can publish your properties, receive reservations in Egyptian Pounds (EGP), and start welcoming travelers from across Egypt and around the world.
                        </div>
                    </div>
                    <p style=""font-size: 14px; color: #475569; margin-bottom: 24px;"">
                        Click the button below to enter your Host Workspace and create your first listing.
                    </p>";
                ctaText = "Enter Host Workspace";
                ctaUrl = "/Host";
            }
            else
            {
                subject = "Havenly Host Application Update";
                title = "Host Application Status";
                contentHtml = $@"
                    <p style=""font-size: 15px; color: #334155; line-height: 1.6; margin-bottom: 20px;"">
                        Thank you for your interest in becoming a host on Havenly. After reviewing your verification document, our administration team was unable to approve your application at this time.
                    </p>
                    {(string.IsNullOrWhiteSpace(notes) ? "" : $@"
                    <div style=""background-color: #fef2f2; border: 1px solid #fecaca; border-radius: 14px; padding: 18px; margin: 20px 0; font-size: 14px; color: #991b1b;"">
                        <strong>Reason provided:</strong> {notes}
                    </div>")}
                    <p style=""font-size: 14px; color: #475569; margin-bottom: 24px;"">
                        If you believe this is a mistake or have an updated government ID / commercial permit, please contact Havenly support.
                    </p>";
                ctaText = "Contact Support";
                ctaUrl = "/Home/Contact";
            }

            string html = BuildHtmlWrapper(title, displayName, contentHtml, ctaText, ctaUrl);
            await SendEmailAsync(hostEmail, subject, html, isHtml: true);
        }

        // Shared HTML Email Template Builder
        private string BuildHtmlWrapper(string heading, string recipientGreeting, string innerHtml, string? ctaText, string? ctaUrl)
        {
            string resolvedUrl = ctaUrl ?? "";
            if (!string.IsNullOrEmpty(resolvedUrl) && !resolvedUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !resolvedUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                resolvedUrl = $"{_baseUrl}/{resolvedUrl.TrimStart('/')}";
            }

            string ctaSection = string.IsNullOrEmpty(ctaText) || string.IsNullOrEmpty(resolvedUrl) ? "" : $@"
                <div style=""text-align: center; margin: 28px 0 16px 0;"">
                    <a href=""{resolvedUrl}"" style=""background-color: #065f46; color: #ffffff; padding: 14px 28px; border-radius: 12px; text-decoration: none; font-weight: 700; font-size: 15px; display: inline-block; box-shadow: 0 2px 8px rgba(6,95,70,0.25);"">
                        {ctaText}
                    </a>
                </div>";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <title>{heading}</title>
</head>
<body style=""font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; background-color: #f8fafc; margin: 0; padding: 30px 10px;"">
    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 560px; background-color: #ffffff; border-radius: 20px; border: 1px solid #e2e8f0; overflow: hidden; box-shadow: 0 4px 20px rgba(0,0,0,0.04);"">
        <!-- Header -->
        <tr>
            <td style=""background-color: #065f46; padding: 26px 32px; text-align: center;"">
                <div style=""display: inline-block; background-color: #ffffff; width: 42px; height: 42px; border-radius: 12px; line-height: 42px; margin-bottom: 6px; box-shadow: 0 2px 6px rgba(0,0,0,0.1);"">
                    <span style=""font-size: 22px; color: #065f46; font-weight: bold;"">⌂</span>
                </div>
                <h1 style=""color: #ffffff; margin: 0; font-size: 20px; font-weight: 700; letter-spacing: -0.5px;"">Havenly</h1>
            </td>
        </tr>
        <!-- Main Content -->
        <tr>
            <td style=""padding: 32px;"">
                <h2 style=""font-size: 21px; color: #0f172a; margin-top: 0; margin-bottom: 16px; font-weight: 700;"">{heading}</h2>
                <p style=""font-size: 15px; color: #475569; margin-top: 0; margin-bottom: 16px;"">
                    Hi {recipientGreeting},
                </p>
                {innerHtml}
                {ctaSection}
            </td>
        </tr>
        <!-- Footer -->
        <tr>
            <td style=""background-color: #f8fafc; padding: 20px 32px; border-top: 1px solid #f1f5f9; text-align: center;"">
                <p style=""font-size: 12px; color: #94a3b8; margin: 0; line-height: 1.5;"">
                    Havenly — Considered homes for short stays.<br/>
                    © 2026 Havenly, Inc. All rights reserved.
                </p>
            </td>
        </tr>
    </table>
</body>
</html>";
        }

        // Legacy helper overloads
        public async Task SendEmailRequestbookingToHost(string toEmail, Booking booking) =>
            await SendBookingRequestToHostAsync(toEmail, "Host", "Guest", $"Property #{booking?.ListingID}", booking?.CheckIn ?? DateTime.Today, booking?.CheckOut ?? DateTime.Today.AddDays(1), booking?.TotalPrice ?? 0, 1, booking?.BookingID ?? 0);

        public async Task ReciveEmailRequestbookingFromHost(string toEmail, Booking booking) =>
            await SendBookingStatusToGuestAsync(toEmail, "Guest", "Host", $"Property #{booking?.ListingID}", booking?.CheckIn ?? DateTime.Today, booking?.CheckOut ?? DateTime.Today.AddDays(1), booking?.TotalPrice ?? 0, "", "", booking?.Status == DAL.Enums.BookingStatus.Approved, booking?.BookingID ?? 0);

        public async Task SendEmailRequestPropertyToAdmin(string toEmail, Booking booking) =>
            await SendListingSubmittedToAdminAsync(toEmail, "Host", "", $"Property #{booking?.ListingID}", "Design homes", "", 0, 0);

        public async Task ReciveEmailRequestPropertyFromAdmin(string toEmail, Booking booking) =>
            await SendListingDecisionToHostAsync(toEmail, "Host", $"Property #{booking?.ListingID}", true, 0);
    }
}

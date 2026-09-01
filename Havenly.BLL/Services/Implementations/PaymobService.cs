using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Havenly.BLL.ModelVMs.Payment;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Havenly.BLL.Services.Implementations
{
    public class PaymobService : IPaymobService
    {
        private readonly HttpClient _httpClient;
        private readonly PaymobSettings _settings;
        private readonly ILogger<PaymobService> _logger;

        public PaymobService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<PaymobService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _settings = new PaymobSettings
            {
                ApiKey = configuration["Paymob:ApiKey"] ?? string.Empty,
                IntegrationId = configuration["Paymob:IntegrationId"] ?? string.Empty,
                IframeId = configuration["Paymob:IframeId"] ?? string.Empty,
                HmacSecret = configuration["Paymob:HmacSecret"] ?? string.Empty,
                IsTestMode = bool.TryParse(configuration["Paymob:IsTestMode"], out var test) ? test : true
            };
        }

        public async Task<string?> CreatePaymentTokenAsync(Booking booking, string guestEmail, string guestName, string guestPhone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.ApiKey))
                {
                    _logger.LogWarning("Paymob ApiKey is not configured. Running in simulated mode.");
                    return null;
                }

                // Step 1: Authentication Request
                var authBody = new { api_key = _settings.ApiKey };
                var authResponse = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/auth/tokens", authBody);
                if (!authResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Paymob Auth Token failed: {Status}", authResponse.StatusCode);
                    return null;
                }

                var authJson = await authResponse.Content.ReadFromJsonAsync<JsonObject>();
                var token = authJson?["token"]?.ToString();
                if (string.IsNullOrEmpty(token)) return null;

                // Step 2: Order Registration
                var amountCents = (int)Math.Round(booking.TotalPrice * 100);
                var orderBody = new
                {
                    auth_token = token,
                    delivery_needed = "false",
                    amount_cents = amountCents.ToString(),
                    currency = "EGP",
                    merchant_order_id = $"BOOKING-{booking.BookingID}-{DateTime.UtcNow.Ticks}"
                };

                var orderResponse = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/ecommerce/orders", orderBody);
                if (!orderResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Paymob Order Registration failed: {Status}", orderResponse.StatusCode);
                    return null;
                }

                var orderJson = await orderResponse.Content.ReadFromJsonAsync<JsonObject>();
                var orderId = orderJson?["id"]?.ToString();
                if (string.IsNullOrEmpty(orderId)) return null;

                // Step 3: Payment Key Generation
                var nameParts = (guestName ?? "Guest User").Trim().Split(' ', 2);
                var firstName = nameParts.Length > 0 && !string.IsNullOrWhiteSpace(nameParts[0]) ? nameParts[0] : "Guest";
                var lastName = nameParts.Length > 1 && !string.IsNullOrWhiteSpace(nameParts[1]) ? nameParts[1] : "Customer";
                var phone = string.IsNullOrWhiteSpace(guestPhone) ? "+201000000000" : guestPhone;

                var paymentKeyBody = new
                {
                    auth_token = token,
                    amount_cents = amountCents.ToString(),
                    expiration = 3600,
                    order_id = orderId,
                    billing_data = new
                    {
                        apartment = "NA",
                        email = string.IsNullOrWhiteSpace(guestEmail) ? "guest@havenly.com" : guestEmail,
                        floor = "NA",
                        first_name = firstName,
                        street = "NA",
                        building = "NA",
                        phone_number = phone,
                        shipping_method = "NA",
                        postal_code = "NA",
                        city = "Cairo",
                        country = "EGY",
                        last_name = lastName,
                        state = "NA"
                    },
                    currency = "EGP",
                    integration_id = int.TryParse(_settings.IntegrationId, out var intId) ? intId : 0
                };

                var paymentKeyResponse = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/acceptance/payment_keys", paymentKeyBody);
                if (!paymentKeyResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Paymob Payment Key Generation failed: {Status}", paymentKeyResponse.StatusCode);
                    return null;
                }

                var paymentKeyJson = await paymentKeyResponse.Content.ReadFromJsonAsync<JsonObject>();
                return paymentKeyJson?["token"]?.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while communicating with Paymob API");
                return null;
            }
        }

        public string GetIframeUrl(string paymentToken)
        {
            var iframeId = !string.IsNullOrWhiteSpace(_settings.IframeId) ? _settings.IframeId : "123456";
            return $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentToken}";
        }

        public bool ValidateHmac(IDictionary<string, string> queryParams)
        {
            if (string.IsNullOrWhiteSpace(_settings.HmacSecret))
                return true; // If secret is not set, allow for test/sandbox development

            if (!queryParams.TryGetValue("hmac", out var receivedHmac) || string.IsNullOrWhiteSpace(receivedHmac))
                return false;

            // Paymob standard concatenated keys in specific order:
            var keys = new[]
            {
                "amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction",
                "id", "integration_id", "is_3d_secure", "is_auth", "is_capture", "is_refunded",
                "is_standalone_payment", "is_voided", "order", "owner", "pending",
                "source_data_pan", "source_data_sub_type", "source_data_type", "success"
            };

            var sb = new StringBuilder();
            foreach (var key in keys)
            {
                if (queryParams.TryGetValue(key, out var val))
                {
                    sb.Append(val);
                }
            }

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_settings.HmacSecret));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
            var computedHmac = Convert.ToHexString(hashBytes).ToLowerInvariant();

            return string.Equals(computedHmac, receivedHmac.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase);
        }
    }
}

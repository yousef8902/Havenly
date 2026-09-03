using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Database;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Havenly.BLL.Services.Implementations
{
    public class PropertyRecommendationEngine : IPropertyRecommendationEngine
    {
        private readonly HavenlyDbContext _context;
        private readonly ILogger<PropertyRecommendationEngine> _logger;

        public PropertyRecommendationEngine(HavenlyDbContext context, ILogger<PropertyRecommendationEngine> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ChatResponseDto> ProcessChatQueryAsync(string userMessage, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                return new ChatResponseDto
                {
                    Success = true,
                    Message = "👋 Hello! How can I help you find the perfect stay in Egypt today?"
                };
            }

            var trimmed = userMessage.Trim();
            var lower = trimmed.ToLowerInvariant();

            // 1. Guard against out-of-scope non-Havenly requests (e.g. war, coding, math, general trivia)
            if (!IsHavenlyRelated(lower))
            {
                return new ChatResponseDto
                {
                    Success = true,
                    Message = "👋 I am Havenly's Travel & Property Concierge!\n\n" +
                              "I specialize exclusively in helping you discover places to stay across Egypt (Cairo, Giza, Dahab, El Gouna, Alexandria, Aswan, Fayoum, etc.) and answering questions about Havenly bookings, payments in EGP, and hosting.\n\n" +
                              "For topics outside Havenly and Egypt hospitality, please consult external resources. How can I help you plan your stay in Egypt?",
                    Metadata = new Dictionary<string, object> { { "IsHandledLocally", true } }
                };
            }

            // 2. Greetings and general introductions
            if (IsGreeting(lower))
            {
                return new ChatResponseDto
                {
                    Success = true,
                    Message = "👋 Welcome to Havenly! I'm your Egyptian travel and stay assistant.\n\n" +
                              "Here are a few things you can ask me:\n" +
                              "• *\"Recommend a beachfront villa in Dahab\"*\n" +
                              "• *\"Find a stay in Giza near the Pyramids\"*\n" +
                              "• *\"Find an apartment in Cairo with Nile views under 600 EGP\"*\n" +
                              "• *\"Where should I stay in El Gouna for 4 guests?\"*\n" +
                              "• *\"How do payments and booking requests work?\"*\n\n" +
                              "Where in Egypt are you looking to stay?",
                    Metadata = new Dictionary<string, object> { { "IsHandledLocally", true } }
                };
            }

            // 3. Platform FAQ & customer support questions
            var faqAnswer = MatchPlatformFaq(lower);
            if (!string.IsNullOrEmpty(faqAnswer))
            {
                return new ChatResponseDto
                {
                    Success = true,
                    Message = faqAnswer,
                    Metadata = new Dictionary<string, object> { { "IsHandledLocally", true } }
                };
            }

            // 4. Property Search & Recommendations
            var recommendationResult = await FindPropertyRecommendationsAsync(trimmed, lower, cancellationToken);
            return recommendationResult;
        }

        private bool IsHavenlyRelated(string text)
        {
            // Explicit greetings & introductions
            string[] greetings = new[] { "hi", "hello", "hey", "salam", "marhaba", "welcome", "good morning", "good evening", "who are you", "what can you do", "help me", "start" };
            if (greetings.Any(g => text == g || text.StartsWith(g + " ") || text.EndsWith(" " + g)))
                return true;

            // Egyptian destinations & landmarks
            string[] egyptKeywords = new[]
            {
                "egypt", "cairo", "giza", "pyramid", "pyramids", "alex", "alexandria", "dahab",
                "sinai", "gouna", "el gouna", "hurghada", "aswan", "luxor", "fayoum", "siwa",
                "sharm", "red sea", "mediterranean", "zamalek", "maadi", "sahel", "matrouh", "nile"
            };
            if (egyptKeywords.Any(k => text.Contains(k)))
                return true;

            // Travel & accommodation keywords
            string[] travelKeywords = new[]
            {
                "stay", "stays", "hotel", "hotels", "villa", "villas", "apartment", "apartments",
                "chalet", "chalets", "flat", "flats", "room", "rooms", "house", "houses", "home", "homes",
                "studio", "studios", "penthouse", "cabin", "cabins", "lodge", "loft", "rent", "rental",
                "book", "booking", "reserve", "reservation", "place", "places", "accommodation", "lodging",
                "trip", "travel", "vacation", "holiday", "night", "nights", "guest", "guests", "host",
                "hosting", "tourism", "tourist", "destination", "visit", "pool", "beach", "sea", "wifi",
                "price", "budget", "cost", "egp", "cheap", "luxury", "bedroom", "bedrooms", "bed", "beds"
            };
            if (travelKeywords.Any(k => text.Contains(k)))
                return true;

            // Platform support & concierge keywords
            string[] platformKeywords = new[]
            {
                "havenly", "paymob", "payment", "pay", "card", "wallet", "review", "reviews",
                "rating", "ratings", "feedback", "cancel", "cancellation", "refund", "login",
                "register", "signup", "account", "verify", "verification", "otp", "admin",
                "recommend", "recommendation", "suggest", "suggestion", "find", "search", "explore",
                "support", "contact", "policy", "features", "fee", "fees"
            };
            if (platformKeywords.Any(k => text.Contains(k)))
                return true;

            return false;
        }

        private bool IsGreeting(string text)
        {
            string[] greetings = new[] { "hi", "hello", "hey", "salam", "marhaba", "good morning", "good evening", "who are you", "what can you do", "help me" };
            return greetings.Any(g => text == g || text.StartsWith(g + " ") || text.EndsWith(" " + g));
        }

        private string? MatchPlatformFaq(string text)
        {
            // If the query is specifically asking to search/recommend places, do not treat as FAQ
            if (text.Contains("recommend") || text.Contains("find") || text.Contains("place") || text.Contains("stay") || text.Contains("villa") || text.Contains("apartment") || text.Contains("chalet") || text.Contains("hotel") || text.Contains("studio"))
            {
                return null;
            }

            if (text.Contains("payment") || text.Contains("paymob") || text.Contains("how to pay") || text.Contains("credit card") || text.Contains("currency"))
            {
                return "💳 **Havenly Payment Information**\n\n" +
                       "• All transactions on Havenly are processed securely in **Egyptian Pounds (EGP)**.\n" +
                       "• We use **Paymob** to support Egyptian & international credit/debit cards as well as local mobile wallets.\n" +
                       "• You are only charged once your host confirms and approves your booking request.";
            }

            if (text.Contains("how to book") || text.Contains("booking process") || text.Contains("reserve a place"))
            {
                return "📅 **How Booking Works on Havenly**\n\n" +
                       "1. **Explore**: Find a home or villa you love and select your check-in/out dates.\n" +
                       "2. **Request**: Click *Request Booking* to send your reservation directly to the host partner.\n" +
                       "3. **Approval & Payment**: Once the host accepts, you'll receive a notification to complete payment in EGP via Paymob.\n" +
                       "4. **Enjoy & Review**: Have a wonderful stay! After checkout, share your feedback with a 1-5 star review.";
            }

            if (text.Contains("become a host") || text.Contains("host registration") || text.Contains("list my property") || text.Contains("verification"))
            {
                return "🏠 **Hosting on Havenly**\n\n" +
                       "• You can register as a Host during sign up by selecting *'I want to host properties'*.\n" +
                       "• For community safety, hosts upload a government ID or permit for admin verification.\n" +
                       "• Once approved, you gain full access to the Host Workspace to publish listings, manage calendars, and earn in EGP!";
            }

            if (text.Contains("review") || text.Contains("rating") || text.Contains("feedback") || text.Contains("host reply"))
            {
                return "⭐ **Reviews & Host Feedback**\n\n" +
                       "• After your stay ends, you will receive an email invitation to rate your experience from 1 to 5 stars.\n" +
                       "• Reviews are published directly on the listing page to assist fellow travelers.\n" +
                       "• Hosts can post official public replies to guest feedback to share hospitality updates!";
            }

            if (text.Contains("cancel") || text.Contains("refund") || text.Contains("cancellation"))
            {
                return "🔄 **Cancellations & Policies**\n\n" +
                       "• You can manage and cancel pending reservations at any time from your *My Bookings* page.\n" +
                       "• For confirmed bookings, cancellation terms depend on host lead time and policy rules.";
            }

            return null;
        }

        private async Task<ChatResponseDto> FindPropertyRecommendationsAsync(string originalText, string lowerText, CancellationToken cancellationToken)
        {
            // Extract Egyptian Destination
            string? detectedCity = null;
            if (lowerText.Contains("giza") || lowerText.Contains("pyramid")) detectedCity = "Giza";
            else if (lowerText.Contains("dahab") || lowerText.Contains("sinai")) detectedCity = "Dahab";
            else if (lowerText.Contains("gouna") || lowerText.Contains("hurghada")) detectedCity = "El Gouna";
            else if (lowerText.Contains("cairo") || lowerText.Contains("zamalek") || lowerText.Contains("maadi") || lowerText.Contains("new cairo") || lowerText.Contains("tagamoa")) detectedCity = "Cairo";
            else if (lowerText.Contains("alex") || lowerText.Contains("alexandria")) detectedCity = "Alexandria";
            else if (lowerText.Contains("aswan") || lowerText.Contains("nubia")) detectedCity = "Aswan";
            else if (lowerText.Contains("luxor")) detectedCity = "Luxor";
            else if (lowerText.Contains("fayoum") || lowerText.Contains("qarun")) detectedCity = "Fayoum";
            else if (lowerText.Contains("siwa")) detectedCity = "Siwa";
            else if (lowerText.Contains("sharm")) detectedCity = "Sharm El Sheikh";
            else if (lowerText.Contains("matrouh") || lowerText.Contains("sahel") || lowerText.Contains("north coast")) detectedCity = "North Coast";

            // If not found in static list, check dynamic cities in DB
            if (string.IsNullOrEmpty(detectedCity))
            {
                var existingCities = await _context.Addresses
                    .Select(a => a.City)
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Distinct()
                    .ToListAsync(cancellationToken);

                foreach (var c in existingCities)
                {
                    if (lowerText.Contains(c.ToLowerInvariant()))
                    {
                        detectedCity = c;
                        break;
                    }
                }
            }

            // Extract Budget
            decimal? maxBudget = null;
            var priceMatch = Regex.Match(lowerText, @"(?:under|less than|below|budget|max|up to)\s*(\d+)", RegexOptions.IgnoreCase);
            if (priceMatch.Success && decimal.TryParse(priceMatch.Groups[1].Value, out decimal parsedBudget))
            {
                maxBudget = parsedBudget;
            }
            else if (lowerText.Contains("budget") || lowerText.Contains("cheap") || lowerText.Contains("affordable"))
            {
                maxBudget = 500m;
            }

            // Extract Guest Count
            int? guestCount = null;
            var guestMatch = Regex.Match(lowerText, @"(\d+)\s*(?:guest|guests|people|person)", RegexOptions.IgnoreCase);
            if (guestMatch.Success && int.TryParse(guestMatch.Groups[1].Value, out int parsedGuests))
            {
                guestCount = parsedGuests;
            }
            else if (lowerText.Contains("couple") || lowerText.Contains("for two"))
            {
                guestCount = 2;
            }
            else if (lowerText.Contains("family"))
            {
                guestCount = 4;
            }

            // Query active approved properties
            var query = _context.Listings
                .Include(l => l.Property)
                    .ThenInclude(p => p.Address)
                .Include(l => l.Property)
                    .ThenInclude(p => p.Images)
                .Where(l => l.ListingStatus == ListingStatus.Approved && !l.Property.IsDeleted);

            if (!string.IsNullOrEmpty(detectedCity))
            {
                var cityLower = detectedCity.ToLowerInvariant();
                query = query.Where(l => 
                    l.Property.Address.City.ToLower().Contains(cityLower) || 
                    l.Property.PropertyName.ToLower().Contains(cityLower) ||
                    l.Property.Description.ToLower().Contains(cityLower));
            }

            if (guestCount.HasValue)
            {
                query = query.Where(l => l.Property.Capacity >= guestCount.Value || l.Property.NumberOfGuests >= guestCount.Value);
            }

            if (maxBudget.HasValue)
            {
                query = query.Where(l => l.Price <= maxBudget.Value);
            }

            // Sort by rating and popularity
            var matchingListings = await query
                .OrderByDescending(l => l.Property.Rating)
                .ThenByDescending(l => l.Property.NumberOfReviews)
                .Take(3)
                .ToListAsync(cancellationToken);

            // Fallback: If strict filters yielded 0 results, relax budget and guest count to still recommend top places in that city (or overall Egypt)
            if (!matchingListings.Any())
            {
                var fallbackQuery = _context.Listings
                    .Include(l => l.Property)
                        .ThenInclude(p => p.Address)
                    .Include(l => l.Property)
                        .ThenInclude(p => p.Images)
                    .Where(l => l.ListingStatus == ListingStatus.Approved && !l.Property.IsDeleted);

                if (!string.IsNullOrEmpty(detectedCity))
                {
                    var cityLower = detectedCity.ToLowerInvariant();
                    fallbackQuery = fallbackQuery.Where(l => 
                        l.Property.Address.City.ToLower().Contains(cityLower) || 
                        l.Property.PropertyName.ToLower().Contains(cityLower) ||
                        l.Property.Description.ToLower().Contains(cityLower));
                }

                matchingListings = await fallbackQuery
                    .OrderByDescending(l => l.Property.Rating)
                    .Take(3)
                    .ToListAsync(cancellationToken);
            }

            var cards = matchingListings.Select(l =>
            {
                var p = l.Property;
                var img = p.Images?.FirstOrDefault(i => i.IsPrimary == true)?.ImagePath
                          ?? p.Images?.FirstOrDefault()?.ImagePath
                          ?? "/images/p1.jpg";

                return new ChatPropertyCardDto
                {
                    PropertyId = p.PropertyID,
                    Title = p.PropertyName,
                    City = p.Address?.City ?? "Egypt",
                    ImageUrl = img,
                    PricePerNight = l.Price,
                    Rating = p.Rating,
                    ReviewCount = (int)p.NumberOfReviews,
                    Capacity = p.Capacity,
                    DetailUrl = $"/Property/Detail/{p.PropertyID}"
                };
            }).ToList();

            string introText;
            if (cards.Any())
            {
                string destinationLabel = !string.IsNullOrEmpty(detectedCity) ? $" in **{detectedCity}**" : " across Egypt";
                string budgetLabel = maxBudget.HasValue ? $" under **{maxBudget.Value:N0} EGP/night**" : "";
                string guestLabel = guestCount.HasValue ? $" for **{guestCount.Value} guest(s)**" : "";

                introText = $"✨ Here are top-rated properties{destinationLabel}{guestLabel}{budgetLabel} curated for you:\n\n";

                foreach (var card in cards)
                {
                    introText += $"• **{card.Title}** — *{card.City}* (⭐ {card.Rating:F1} · {card.PricePerNight:N0} EGP/night)\n";
                }

                introText += "\nClick on any card below to view the full photo gallery, amenities, and request a stay!";
            }
            else
            {
                introText = "I couldn't find active listings matching your exact criteria right now. You can explore all featured homes on our **Explore** page or try searching for popular spots like Dahab, El Gouna, or Cairo!";
            }

            return new ChatResponseDto
            {
                Success = true,
                Message = introText,
                RecommendedProperties = cards,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}

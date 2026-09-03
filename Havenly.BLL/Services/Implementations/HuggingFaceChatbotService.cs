using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Havenly.BLL.Services.Implementations
{
    /// <summary>
    /// Implementation of IChatbotService using Hugging Face's OpenAI-compatible API.
    /// Handles communication with Hugging Face models via HTTP for customer support.
    /// </summary>
    public class HuggingFaceChatbotService : IChatbotService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HuggingFaceChatbotService> _logger;
        private readonly IPropertyRecommendationEngine _recommendationEngine;

        private readonly string _apiKey;
        private readonly string _apiEndpoint;
        private readonly string _model;
        private readonly int _timeoutSeconds;

        public HuggingFaceChatbotService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<HuggingFaceChatbotService> logger,
            IPropertyRecommendationEngine recommendationEngine)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recommendationEngine = recommendationEngine ?? throw new ArgumentNullException(nameof(recommendationEngine));

            // Load configuration safely from appsettings.json
            _apiKey = _configuration["HuggingFace:ApiKey"] ?? string.Empty;
            _apiEndpoint = _configuration["HuggingFace:ApiEndpoint"] ?? "https://router.huggingface.co/v1/chat/completions";
            _model = _configuration["HuggingFace:Model"] ?? "meta-llama/Llama-3.2-3B-Instruct";

            if (!int.TryParse(_configuration["HuggingFace:TimeoutSeconds"], out _timeoutSeconds))
            {
                _timeoutSeconds = 30; // Default timeout of 30 seconds
            }
        }

        /// <summary>
        /// Sends a message to the chatbot, using catalog intelligence and LLM enrichment when available.
        /// </summary>
        public async Task<ChatResponseDto> SendMessageAsync(ChatRequestDto request, CancellationToken cancellationToken = default)
        {
            ChatResponseDto? localResult = null;
            try
            {
                // Validate input
                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (string.IsNullOrWhiteSpace(request.Message))
                    return new ChatResponseDto
                    {
                        Success = false,
                        Error = "Message cannot be empty.",
                        Message = string.Empty
                    };

                // Run recommendation engine first to extract intent, answer FAQs, or search live database
                localResult = await _recommendationEngine.ProcessChatQueryAsync(request.Message, cancellationToken);

                // If marked as handled locally (out-of-scope question, greeting, or platform FAQ), return immediately!
                if (localResult.Metadata != null && localResult.Metadata.ContainsKey("IsHandledLocally"))
                {
                    return localResult;
                }

                // If no external API key is provided, return intelligent catalog response immediately
                if (string.IsNullOrWhiteSpace(_apiKey))
                {
                    return localResult;
                }

                // Build grounded prompt for customer support & Egypt property concierge persona
                var catalogSummary = (localResult.RecommendedProperties != null && localResult.RecommendedProperties.Any())
                    ? string.Join("; ", localResult.RecommendedProperties.Select(p => $"'{p.Title}' in {p.City} at {p.PricePerNight:N0} EGP/night (Rating {p.Rating:F1})"))
                    : "No specific property match";

                var systemPrompt = $@"You are Havenly's Egyptian Property & Travel Concierge. 
Havenly is a trusted vacation rental platform across Egypt.
MATCHING PROPERTIES RETRIEVED FROM HAVENLY DATABASE:
{catalogSummary}

CRITICAL RULES:
1. ONLY answer questions about Havenly, accommodations across Egypt, booking, payments in EGP, and hosting.
2. When properties are listed above, recommend them enthusiastically by name and highlight their price in EGP and city.
3. NEVER claim Havenly has no listings or tell users to look elsewhere when properties are listed above!
4. Keep answers concise, warm, helpful, and under 3 sentences.";

                // Prepare the request payload for Hugging Face OpenAI-compatible API
                var payload = new
                {
                    model = _model,
                    messages = new object[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = request.Message }
                    },
                    temperature = 0.7,
                    max_tokens = 512,
                    top_p = 0.9
                };

                using (var client = _httpClientFactory.CreateClient())
                {
                    // Set authentication header with Bearer token
                    client.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

                    client.Timeout = TimeSpan.FromSeconds(_timeoutSeconds);

                    // Log the request details for debugging
                    _logger.LogInformation("Sending request to Hugging Face API. Model: {Model}, Endpoint: {Endpoint}", _model, _apiEndpoint);

                    // Send POST request to Hugging Face API
                    var response = await client.PostAsJsonAsync(
                        _apiEndpoint,
                        payload,
                        cancellationToken);

                    // Log the response status
                    _logger.LogInformation("Hugging Face API Response: {StatusCode}", response.StatusCode);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                        _logger.LogWarning("Hugging Face API returned error ({StatusCode}), falling back to local recommendations: {Content}", response.StatusCode, errorContent);
                        return localResult;
                    }

                    // Parse the response
                    var content = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);

                    // Extract the assistant's message from the response
                    if (content.TryGetProperty("choices", out var choices) && 
                        choices.GetArrayLength() > 0)
                    {
                        var firstChoice = choices[0];
                        if (firstChoice.TryGetProperty("message", out var message) &&
                            message.TryGetProperty("content", out var contentProp))
                        {
                            var assistantMessage = contentProp.GetString() ?? string.Empty;

                            _logger.LogInformation("Successfully retrieved response from Hugging Face API.");

                            // Guard against hallucinated negative claims when real properties exist
                            if (localResult.RecommendedProperties != null && localResult.RecommendedProperties.Any())
                            {
                                var lowerMsg = assistantMessage.ToLowerInvariant();
                                if (lowerMsg.Contains("doesn't have") || lowerMsg.Contains("does not have") || lowerMsg.Contains("don't have") || lowerMsg.Contains("no specific listing"))
                                {
                                    _logger.LogWarning("LLM hallucinated lack of listings; keeping accurate local recommendation text.");
                                    return localResult;
                                }
                            }

                            localResult.Message = assistantMessage;
                            return localResult;
                        }
                    }

                    return localResult;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "External chatbot API call failed, falling back to local recommendation engine.");
                return localResult ?? new ChatResponseDto
                {
                    Success = true,
                    Message = "👋 I am Havenly's travel assistant! How can I help you discover places to stay across Egypt today?"
                };
            }
        }
    }
}

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

        private readonly string _apiKey;
        private readonly string _apiEndpoint;
        private readonly string _model;
        private readonly int _timeoutSeconds;

        public HuggingFaceChatbotService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<HuggingFaceChatbotService> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Load configuration from appsettings.json
            _apiKey = _configuration["HuggingFace:ApiKey"] ?? 
                throw new InvalidOperationException("HuggingFace:ApiKey is not configured.");
            _apiEndpoint = _configuration["HuggingFace:ApiEndpoint"] ?? 
                throw new InvalidOperationException("HuggingFace:ApiEndpoint is not configured.");
            _model = _configuration["HuggingFace:Model"] ?? 
                throw new InvalidOperationException("HuggingFace:Model is not configured.");

            if (!int.TryParse(_configuration["HuggingFace:TimeoutSeconds"], out _timeoutSeconds))
            {
                _timeoutSeconds = 30; // Default timeout of 30 seconds
            }
        }

        /// <summary>
        /// Sends a message to the Hugging Face API and returns the chatbot response.
        /// </summary>
        public async Task<ChatResponseDto> SendMessageAsync(ChatRequestDto request, CancellationToken cancellationToken = default)
        {
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

                // Build system prompt for customer support persona
                var systemPrompt = @"You are a helpful and professional customer support assistant for Havenly, a property rental platform. 
Your role is to assist users with questions about bookings, properties, payments, and platform features. 
Keep responses concise, polite, and focused on helping the customer. 
If you don't know something, offer to connect them with a human agent.";

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
                        _logger.LogError("Hugging Face API Error: {StatusCode} - {Content}", response.StatusCode, errorContent);

                        return new ChatResponseDto
                        {
                            Success = false,
                            Error = $"API error (Status {response.StatusCode}): {errorContent}",
                            Message = string.Empty
                        };
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

                            return new ChatResponseDto
                            {
                                Success = true,
                                Message = assistantMessage,
                                Timestamp = DateTime.UtcNow,
                                Metadata = new Dictionary<string, object>
                                {
                                    { "model", _model },
                                    { "provider", "HuggingFace" }
                                }
                            };
                        }
                    }

                    _logger.LogWarning("Unexpected response format from Hugging Face API.");
                    return new ChatResponseDto
                    {
                        Success = false,
                        Error = "Unexpected response format from the API.",
                        Message = string.Empty
                    };
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Request to Hugging Face API was cancelled: {Message}", ex.Message);
                return new ChatResponseDto
                {
                    Success = false,
                    Error = "Request timed out. Please try again.",
                    Message = string.Empty
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("HTTP error communicating with Hugging Face API: {Message}", ex.Message);
                return new ChatResponseDto
                {
                    Success = false,
                    Error = "Failed to connect to the chatbot service. Please try again later.",
                    Message = string.Empty
                };
            }
            catch (JsonException ex)
            {
                _logger.LogError("Error deserializing response from Hugging Face API: {Message}", ex.Message);
                return new ChatResponseDto
                {
                    Success = false,
                    Error = "Error processing the response. Please try again.",
                    Message = string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in HuggingFaceChatbotService.SendMessageAsync");
                return new ChatResponseDto
                {
                    Success = false,
                    Error = "An unexpected error occurred. Please try again later.",
                    Message = string.Empty
                };
            }
        }
    }
}

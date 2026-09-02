using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Havenly.PL.Controllers
{
    /// <summary>
    /// Controller for handling customer support chatbot interactions.
    /// Provides JSON endpoints for sending messages and receiving responses.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatbotService _chatbotService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatbotService chatbotService, ILogger<ChatController> logger)
        {
            _chatbotService = chatbotService ?? throw new ArgumentNullException(nameof(chatbotService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Sends a message to the chatbot and receives a response.
        /// </summary>
        /// <param name="request">The chat request containing the user's message.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A ChatResponseDto with the chatbot's response or error information.</returns>
        [HttpPost("send-message")]
        public async Task<ActionResult<ChatResponseDto>> SendMessage(
            [FromBody] ChatRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Validate the request
                if (request == null)
                {
                    _logger.LogWarning("SendMessage received null request.");
                    return BadRequest(new ChatResponseDto
                    {
                        Success = false,
                        Error = "Request body cannot be null.",
                        Message = string.Empty
                    });
                }

                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    _logger.LogWarning("SendMessage received empty message.");
                    return BadRequest(new ChatResponseDto
                    {
                        Success = false,
                        Error = "Message cannot be empty.",
                        Message = string.Empty
                    });
                }

                // Log the incoming request (without sensitive data)
                _logger.LogInformation("Chat message received. Message length: {Length}", request.Message.Length);

                // Send the message to the chatbot service
                var response = await _chatbotService.SendMessageAsync(request, cancellationToken);

                // Return the response
                if (response.Success)
                {
                    _logger.LogInformation("Chat message processed successfully.");
                    return Ok(response);
                }
                else
                {
                    _logger.LogWarning("Chat service returned error: {Error}", response.Error);
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "Request was cancelled.");
                return StatusCode(StatusCodes.Status408RequestTimeout, new ChatResponseDto
                {
                    Success = false,
                    Error = "Request timed out. Please try again.",
                    Message = string.Empty
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in SendMessage endpoint.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ChatResponseDto
                {
                    Success = false,
                    Error = "An unexpected error occurred. Please try again later.",
                    Message = string.Empty
                });
            }
        }

        /// <summary>
        /// Health check endpoint for the chatbot service.
        /// </summary>
        [HttpGet("health")]
        [AllowAnonymous]
        public ActionResult<object> Health()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                service = "Chatbot"
            });
        }
    }
}

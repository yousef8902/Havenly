using Havenly.BLL.ModelVMs;

namespace Havenly.BLL.Services.Abstractions
{
    /// <summary>
    /// Service interface for chatbot operations.
    /// Defines contract for sending messages and retrieving chatbot responses.
    /// </summary>
    public interface IChatbotService
    {
        /// <summary>
        /// Sends a message to the chatbot and retrieves a response.
        /// </summary>
        /// <param name="request">The chat request containing the user's message.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A ChatResponseDto containing the chatbot's response or error information.</returns>
        Task<ChatResponseDto> SendMessageAsync(ChatRequestDto request, CancellationToken cancellationToken = default);
    }
}

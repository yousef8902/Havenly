using System.Threading;
using System.Threading.Tasks;
using Havenly.BLL.ModelVMs;

namespace Havenly.BLL.Services.Abstractions
{
    /// <summary>
    /// Engine for intelligent property recommendations, platform FAQ answering,
    /// and out-of-scope question gating.
    /// </summary>
    public interface IPropertyRecommendationEngine
    {
        /// <summary>
        /// Analyzes the user's natural language input, performs catalog search / FAQ matching,
        /// and returns conversational text along with interactive property cards.
        /// </summary>
        Task<ChatResponseDto> ProcessChatQueryAsync(string userMessage, CancellationToken cancellationToken = default);
    }
}

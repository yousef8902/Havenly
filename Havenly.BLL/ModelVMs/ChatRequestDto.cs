namespace Havenly.BLL.ModelVMs
{
    /// <summary>
    /// DTO for incoming chat message requests from the client.
    /// </summary>
    public class ChatRequestDto
    {
        /// <summary>
        /// The user's message/question for the chatbot.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Optional conversation context for multi-turn interactions.
        /// </summary>
        public string? ConversationContext { get; set; }
    }
}

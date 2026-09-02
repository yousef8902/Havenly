namespace Havenly.BLL.ModelVMs
{
    /// <summary>
    /// DTO for outgoing chat responses from the server.
    /// </summary>
    public class ChatResponseDto
    {
        /// <summary>
        /// Indicates whether the API call was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The chatbot's response message.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Error message if the request failed.
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// Timestamp of the response.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Optional metadata about the response (e.g., model used, tokens consumed).
        /// </summary>
        public Dictionary<string, object>? Metadata { get; set; }
    }
}

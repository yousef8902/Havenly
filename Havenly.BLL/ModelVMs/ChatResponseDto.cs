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

        /// <summary>
        /// Optional list of recommended property cards matching the user's query.
        /// </summary>
        public List<ChatPropertyCardDto>? RecommendedProperties { get; set; }
    }

    /// <summary>
    /// Mini property card representation for chatbot recommendations.
    /// </summary>
    public class ChatPropertyCardDto
    {
        public long PropertyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public int Capacity { get; set; }
        public string DetailUrl { get; set; } = string.Empty;
    }
}

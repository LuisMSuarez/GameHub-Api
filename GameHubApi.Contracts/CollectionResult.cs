namespace GameHubApi.Contracts
{
    // Enables JSON property name mapping for serialization/deserialization
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a paginated collection of results returned from an API endpoint.
    /// Generic type <typeparamref name="T"/> allows flexibility for different data models.
    /// </summary>
    public class CollectionResult<T>
    {
        /// <summary>
        /// Total number of items available across all pages.
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }

        /// <summary>
        /// URL to the next page of results, if available.
        /// Null if there are no further pages.
        /// </summary>
        [JsonPropertyName("next")]
        public string? Next { get; set; }

        /// <summary>
        /// URL to the previous page of results, if available.
        /// Null if this is the first page.
        /// </summary>
        [JsonPropertyName("previous")]
        public string? Previous { get; set; }

        /// <summary>
        /// List of results for the current page.
        /// Initialized to an empty list to avoid null reference issues.
        /// </summary>
        [JsonPropertyName("results")]
        public List<T> Results { get; set; } = new List<T>();
    }
}

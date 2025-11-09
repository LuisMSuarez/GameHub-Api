namespace GameHubApi.Contracts
{
    // Enables JSON property name mapping for serialization/deserialization
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a descriptive tag used to categorize or label games in the GameHub API.
    /// Tags help with filtering, search, and content discovery.
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Unique identifier for the tag.
        /// </summary>
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        /// <summary>
        /// Human-readable name of the tag (e.g., "Multiplayer", "Open World").
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// URL-friendly identifier for the tag (typically lowercase with hyphens).
        /// Useful for routing, filtering, or SEO.
        /// </summary>
        [JsonPropertyName("slug")]
        public required string Slug { get; set; }
    }
}

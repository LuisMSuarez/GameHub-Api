namespace GameHubApi.Contracts
{
    // Enables JSON property name mapping for serialization/deserialization
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a gaming platform (e.g., PC, PlayStation, Xbox) in the GameHub API.
    /// Used to categorize and filter games by their supported systems.
    /// </summary>
    public class Platform
    {
        /// <summary>
        /// Unique identifier for the platform.
        /// </summary>
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        /// <summary>
        /// Human-readable name of the platform (e.g., "PlayStation 5").
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// URL-friendly identifier for the platform (typically lowercase with hyphens).
        /// Useful for routing or filtering in frontend applications.
        /// </summary>
        [JsonPropertyName("slug")]
        public required string Slug { get; set; }
    }
}

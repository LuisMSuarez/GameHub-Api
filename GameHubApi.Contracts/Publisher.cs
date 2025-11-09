namespace GameHubApi.Contracts
{
    // Enables JSON property name mapping for serialization/deserialization
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a game publisher in the GameHub API.
    /// Used to identify and display metadata about companies that publish games.
    /// </summary>
    public class Publisher
    {
        /// <summary>
        /// Unique identifier for the publisher.
        /// </summary>
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        /// <summary>
        /// Display name of the publisher (e.g., "Ubisoft", "Nintendo").
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// URL-friendly identifier for the publisher (typically lowercase with hyphens).
        /// Useful for routing, filtering, or SEO.
        /// </summary>
        [JsonPropertyName("slug")]
        public required string Slug { get; set; }

        /// <summary>
        /// Optional background image associated with the publisher.
        /// Often used for UI presentation or branding.
        /// </summary>
        [JsonPropertyName("image_background")]
        public string? BackgroundImage { get; set; }
    }
}

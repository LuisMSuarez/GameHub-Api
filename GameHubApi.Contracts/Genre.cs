namespace GameHubApi.Contracts
{
    // Enables JSON property name mapping for serialization/deserialization
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a game genre used in the GameHub API.
    /// Includes metadata for display and filtering purposes.
    /// </summary>
    public class Genre
    {
        /// <summary>
        /// Unique identifier for the genre.
        /// </summary>
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        /// <summary>
        /// Display name of the genre (e.g., "Action", "Puzzle").
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// URL-friendly identifier for the genre (typically lowercase with hyphens).
        /// </summary>
        [JsonPropertyName("slug")]
        public required string Slug { get; set; }

        /// <summary>
        /// Optional background image associated with the genre, used for UI presentation.
        /// </summary>
        [JsonPropertyName("image_background")]
        public string? BackgroundImage { get; set; }
    }
}

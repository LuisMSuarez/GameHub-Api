namespace GameHubApi.Contracts
{
    // Enables JSON property name mapping for serialization/deserialization
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a movie entity associated with a game or media item in the GameHub API.
    /// Includes metadata for display and playback purposes.
    /// </summary>
    public class Movie
    {
        /// <summary>
        /// Unique identifier for the movie.
        /// </summary>
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        /// <summary>
        /// Display name or title of the movie.
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// URL or path to a preview clip or thumbnail for the movie.
        /// </summary>
        [JsonPropertyName("preview")]
        public required string Preview { get; set; }

        /// <summary>
        /// Arbitrary key-value metadata associated with the movie.
        /// Can include runtime, resolution, format, or other descriptive attributes.
        /// </summary>
        [JsonPropertyName("data")]
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
    }
}

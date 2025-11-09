namespace GameHubApi.Contracts
{
    // Enables JSON property name mapping for serialization/deserialization
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a screenshot associated with a game or media item in the GameHub API.
    /// </summary>
    public class Screenshot
    {
        /// <summary>
        /// Unique identifier for the screenshot.
        /// </summary>
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        /// <summary>
        /// URL or path to the screenshot image file.
        /// </summary>
        [JsonPropertyName("image")]
        public required string Image { get; set; }

        /// <summary>
        /// Width of the screenshot in pixels.
        /// </summary>
        [JsonPropertyName("width")]
        public required int Width { get; set; }

        /// <summary>
        /// Height of the screenshot in pixels.
        /// </summary>
        [JsonPropertyName("height")]
        public required int Height { get; set; }

        /// <summary>
        /// Indicates whether the screenshot has been marked as deleted.
        /// Useful for soft deletion or archival logic.
        /// </summary>
        [JsonPropertyName("is_deleted")]
        public required bool IsDeleted { get; set; }
    }
}

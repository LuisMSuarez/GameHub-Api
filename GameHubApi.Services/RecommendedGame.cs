using System.Text.Json.Serialization;

namespace GameHubApi.Services
{
    /// <summary>
    /// Represents a simplified game recommendation returned by the AI model.
    /// </summary>
    internal class RecommendedGame
    {
        /// <summary>
        /// Gets or sets the display name of the recommended game.
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the slug identifier used to fetch full game details.
        /// </summary>
        [JsonPropertyName("slug")]
        public required string Slug { get; set; }
    }
}

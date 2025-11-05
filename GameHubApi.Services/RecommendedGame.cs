using System.Text.Json.Serialization;

namespace GameHubApi.Services
{
    internal class RecommendedGame
    {

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("slug")]
        public required string Slug { get; set; }
    }
}

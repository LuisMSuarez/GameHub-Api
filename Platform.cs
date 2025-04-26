using System.Text.Json.Serialization;

namespace GameHubApi
{
    public class Platform
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("slug")]
        public required string Slug { get; set; }
    }
}

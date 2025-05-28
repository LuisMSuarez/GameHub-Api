using System.Text.Json.Serialization;

namespace GameHubApi.Contracts
{
    public class Genre
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("slug")]
        public required string Slug { get; set; }

        [JsonPropertyName("image_background")]
        public string? BackgroundImage { get; set; }
    }
}

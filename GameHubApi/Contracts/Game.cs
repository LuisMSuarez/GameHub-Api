namespace GameHubApi.Contracts
{
    using System.Text.Json.Serialization;

    public class Game
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("slug")]
        public required string Slug { get; set; }

        [JsonPropertyName("background_image")]
        public string? BackgroundImage { get; set; }

        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        [JsonPropertyName("metacritic")]
        public int? Metacritic { get; set; }

        [JsonPropertyName("rating_top")]
        public int RatingTop { get; set; }

        [JsonPropertyName("parent_platforms")]
        public IList<ParentPlatform> ParentPlatforms { get; set; } = new List<ParentPlatform>();

        [JsonPropertyName("tags")]
        public IList<Tag> Tags { get; set; } = new List<Tag>();

        [JsonPropertyName("description_raw")]
        public string? Description { get; set; }
    }
}

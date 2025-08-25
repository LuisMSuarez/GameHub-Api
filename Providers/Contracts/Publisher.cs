namespace GameHubApi.Providers.Contracts
{
    using System.Text.Json.Serialization;
    public class Publisher
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("slug")]
        public required string Slug { get; set; }

        [JsonPropertyName("image_background")]
        public string? BackgroundImage { get; set; }
    }
}

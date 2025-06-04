namespace GameHubApi.Contracts
{
    using System.Text.Json.Serialization;
    public class Platform
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("slug")]
        public required string Slug { get; set; }
    }
}

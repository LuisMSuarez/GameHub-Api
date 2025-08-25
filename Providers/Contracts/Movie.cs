namespace GameHubApi.Providers.Contracts
{
    using System.Text.Json.Serialization;
    public class Movie
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("preview")]
        public required string Preview { get; set; }

        [JsonPropertyName("data")]
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
    }
}

namespace GameHubApi.Providers.Contracts
{
    using System.Text.Json.Serialization;
    public class Screenshot
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [JsonPropertyName("image")]
        public required string Image { get; set; }

        [JsonPropertyName("width")]
        public required int Width { get; set; }

        [JsonPropertyName("height")]
        public required int Height { get; set; }

        [JsonPropertyName("is_deleted")]
        public required bool IsDeleted { get; set; }
    }
}

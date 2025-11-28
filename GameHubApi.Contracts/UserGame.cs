using System.Text.Json.Serialization;

namespace GameHubApi.Contracts
{
    public class UserGame
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("userId")]
        public required string UserId { get; set; }

        [JsonPropertyName("gameId")]
        public required string GameId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("preferences")]
        public Preference Preferences { get; set; }
    }
}

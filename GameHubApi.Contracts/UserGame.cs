using System.Text.Json.Serialization;

namespace GameHubApi.Contracts
{
    public class UserGame
    {
        public required string UserId { get; set; }
        public required string GameId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Preference Preference { get; set; }
    }
}

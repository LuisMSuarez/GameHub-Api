using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameHubApi.Repository.Contracts
{
    internal class UserGame
    {
        [JsonProperty("id")]
        public required string Id { get; set; }

        [JsonProperty("userId")]
        public required string UserId { get; set; }

        [JsonProperty("gameId")]
        public required string GameId { get; set; }

        [JsonProperty("preferences")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Preference Preferences { get; set; } 
    }
}

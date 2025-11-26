using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameHubApi.Repository.Contracts
{
    internal class UserGame
    {
        [JsonProperty("userId")]
        public required string UserId { get; set; }

        [JsonProperty("gameId")]
        public required string GameId { get; set; }

        [JsonProperty("preference")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Preference Preference { get; set; } 
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameHubApi.Repository.Entities
{
    internal class UserGame
    {
        [JsonProperty("id")]
        public required string Id { get; set; }

        [JsonProperty("slug")]
        public required string Slug { get; set; }

        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("background_image")]
        public string? BackgroundImage { get; set; }

        [JsonProperty("userId")]
        public required string UserId { get; set; }

        [JsonProperty("gameId")]
        public required int GameId { get; set; }

        [JsonProperty("preferences")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Preference Preferences { get; set; } 
    }
}

using System.Text.Json.Serialization;

namespace GameHubApi
{
    public class ParentPlatform
    {
        [JsonPropertyName("platform")]
        public required Platform Platform { get; set; }
    }
}

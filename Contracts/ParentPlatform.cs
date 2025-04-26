using System.Text.Json.Serialization;

namespace GameHubApi.Contracts
{
    public class ParentPlatform
    {
        [JsonPropertyName("platform")]
        public required Platform Platform { get; set; }
    }
}

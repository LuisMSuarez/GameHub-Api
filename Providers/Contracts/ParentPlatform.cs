namespace GameHubApi.Providers.Contracts
{
    using System.Text.Json.Serialization;
    public class ParentPlatform
    {
        [JsonPropertyName("platform")]
        public required Platform Platform { get; set; }
    }
}

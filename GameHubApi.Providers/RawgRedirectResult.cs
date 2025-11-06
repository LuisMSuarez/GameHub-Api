using System.Text.Json.Serialization;

namespace GameHubApi.Providers
{
    internal class RawgRedirectResult
    {
        [JsonPropertyName("redirect")]
        public required bool Redirect { get; set; }

        [JsonPropertyName("slug")]
        public required string Slug { get; set; }
    }
}

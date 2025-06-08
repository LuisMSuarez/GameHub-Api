using System.Text.Json.Serialization;

namespace GameHubApi.Providers
{
    public class TranslationResult
    {
        [JsonPropertyName("detectedLanguage")]
        public DetectedLanguage? DetectedLanguage { get; set; }

        [JsonPropertyName("translations")]
        public IList<Translation> Translations { get; set; } = new List<Translation>();
    }

    public class DetectedLanguage
    {
        [JsonPropertyName("language")]
        public required string Language { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }
    }

    public class Translation
    {
        [JsonPropertyName("text")]
        public required string Text { get; set; }

        [JsonPropertyName("to")]
        public required string To { get; set; }
    }
}

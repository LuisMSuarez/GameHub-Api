using System.Text.Json.Serialization;

namespace GameHubApi.Providers
{
    /// <summary>
    /// Represents the result of a translation request, including detected language and translated text(s).
    /// </summary>
    public class TranslationResult
    {
        /// <summary>
        /// The language detected from the input text, if available.
        /// </summary>
        [JsonPropertyName("detectedLanguage")]
        public DetectedLanguage? DetectedLanguage { get; set; }

        /// <summary>
        /// A list of translations returned by the API.
        /// </summary>
        [JsonPropertyName("translations")]
        public IList<Translation> Translations { get; set; } = new List<Translation>();
    }

    /// <summary>
    /// Represents the detected language and its confidence score.
    /// </summary>
    public class DetectedLanguage
    {
        /// <summary>
        /// The ISO code of the detected language (e.g., "en" for English).
        /// </summary>
        [JsonPropertyName("language")]
        public required string Language { get; set; }

        /// <summary>
        /// The confidence score of the detected language, ranging from 0.0 to 1.0.
        /// </summary>
        [JsonPropertyName("score")]
        public double Score { get; set; }
    }

    /// <summary>
    /// Represents a single translated text and its target language.
    /// </summary>
    public class Translation
    {
        /// <summary>
        /// The translated text.
        /// </summary>
        [JsonPropertyName("text")]
        public required string Text { get; set; }

        /// <summary>
        /// The target language code (e.g., "fr" for French).
        /// </summary>
        [JsonPropertyName("to")]
        public required string To { get; set; }
    }
}

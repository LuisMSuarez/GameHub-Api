namespace GameHubApi.Providers
{
    public class TranslationResultCollection
    {
        public IList<TranslationResult> Translations { get; set; } = new List<TranslationResult>();
    }

    public class TranslationResult
    {
        public DetectedLanguage? DetectedLanguage { get; set; }
        public IList<Translation> Translations { get; set; } = new List<Translation>();
    }

    public class DetectedLanguage
    {
        public string? Language { get; set; }
        public double Score { get; set; }
    }

    public class Translation
    {
        public string? Text { get; set; }
        public string? To { get; set; }
        public DetectedLanguage? DetectedLanguage { get; set; }
    }
}

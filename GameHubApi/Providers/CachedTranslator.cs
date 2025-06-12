
using GameHubApi.Contracts;

namespace GameHubApi.Providers
{
    public class CachedTranslator : ITranslator
    {
        private readonly ILruCache<string, string> translationCache;
        private readonly ITranslator azureTranslator;
        public CachedTranslator(Func<string, ITranslator> translatorFactory, ILruCache<string, string> translationCache)
        {
            if (translatorFactory == null)
            {
                throw new ArgumentNullException(nameof(translatorFactory));
            }
            this.translationCache = translationCache ?? throw new ArgumentNullException(nameof(translationCache));

            this.azureTranslator = translatorFactory("Base") ?? throw new ArgumentNullException(nameof(translatorFactory));
        }
        public async Task<string> Translate(string text, string? from, string to)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text cannot be null or empty.", nameof(text));
            }
            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentException("Target language cannot be null or empty.", nameof(to));
            }
            var cacheKey = $"{text}_{from}_{to}".GetHashCode().ToString("X8");
            var cachedTranslation = this.translationCache.Get(cacheKey);
            if (cachedTranslation != null)
            {
                return cachedTranslation;
            }
            var translation = await this.azureTranslator.Translate(text, from, to);
            this.translationCache.Set(cacheKey, translation, TimeSpan.FromDays(7));
            return translation;
        }
    }
}

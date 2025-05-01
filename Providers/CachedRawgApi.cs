namespace GameHubApi.Providers
{
    using GameHubApi.Contracts;
    using System.Text;

    public class CachedRawgApi : IRawgApi
    {
        private readonly ILruCache<string, CollectionResult<Game>> gamesCache;
        private readonly ILogger<CachedRawgApi> logger;
        private readonly IRawgApi rawgApi;

        public CachedRawgApi(ILruCache<string, CollectionResult<Game>> gamesCache, ILogger<CachedRawgApi> logger, Func<string, IRawgApi> rawgApiFactory)
        {
            this.gamesCache = gamesCache ?? throw new ArgumentNullException(nameof(gamesCache));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (rawgApiFactory == null)
            {
                throw new ArgumentNullException(nameof(rawgApiFactory));
            }

            this.rawgApi = rawgApiFactory("Base") ?? throw new ArgumentNullException(nameof(rawgApiFactory));
        }
        public async Task<CollectionResult<Game>> GetGamesAsync(string? genres, string? parentPlatforms, string? ordering, string? search, int page = 1, int pageSize = 20)
        {
            var cacheKey = BuildCacheKey(genres, parentPlatforms, ordering, search, page, pageSize);
            var cachedResponse = this.gamesCache.Get(cacheKey);

            if (cachedResponse != null)
            {
                this.logger.LogInformation($"Cache hit for request URI: {cacheKey}");
                return cachedResponse;
            }

            // fallback to call base service
            var results = await this.rawgApi.GetGamesAsync(genres, parentPlatforms, ordering, search, page, pageSize);

            this.gamesCache.Set(cacheKey, results, TimeSpan.FromDays(7));
            this.logger.LogInformation($"Cache miss for request URI: {cacheKey}");

            return results;
        }

        private string BuildCacheKey(string? genres, string? parentPlatforms, string? ordering, string? search, int page, int pageSize)
        {
            var cacheKeyBuilder = new StringBuilder($"/games&page={page}&page_size={pageSize}");

            if (!string.IsNullOrWhiteSpace(genres))
            {
                cacheKeyBuilder.Append($"&genres={Uri.EscapeDataString(genres)}");
            }
            if (!string.IsNullOrWhiteSpace(parentPlatforms))
            {
                cacheKeyBuilder.Append($"&parent_platforms={Uri.EscapeDataString(parentPlatforms)}");
            }
            if (!string.IsNullOrWhiteSpace(ordering))
            {
                cacheKeyBuilder.Append($"&ordering={Uri.EscapeDataString(ordering)}");
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                cacheKeyBuilder.Append($"&search={Uri.EscapeDataString(search)}");
            }

            return cacheKeyBuilder.ToString();
        }
    }
}

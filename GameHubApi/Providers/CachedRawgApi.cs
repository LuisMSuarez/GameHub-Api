namespace GameHubApi.Providers
{
    using GameHubApi.Contracts;
    using System.Text;

    public class CachedRawgApi : IRawgApi
    {
        private readonly ILruCache<string, CollectionResult<Game>> gamesCache;
        private readonly ILruCache<string, CollectionResult<Genre>> genresCache;
        private readonly ILogger<CachedRawgApi> logger;
        private readonly IRawgApi rawgApi;

        public CachedRawgApi(ILruCache<string, CollectionResult<Game>> gamesCache, ILruCache<string, CollectionResult<Genre>> genresCache, ILogger<CachedRawgApi> logger, Func<string, IRawgApi> rawgApiFactory)
        {
            this.gamesCache = gamesCache ?? throw new ArgumentNullException(nameof(gamesCache));
            this.genresCache = genresCache ?? throw new ArgumentNullException(nameof(genresCache));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (rawgApiFactory == null)
            {
                throw new ArgumentNullException(nameof(rawgApiFactory));
            }

            this.rawgApi = rawgApiFactory("Base") ?? throw new ArgumentNullException(nameof(rawgApiFactory));
        }
        public async Task<CollectionResult<Game>> GetGamesAsync(string? genres, string? parentPlatforms, string? ordering, string? search, int page = 1, int pageSize = 20)
        {
            var cacheKey = BuildCacheKey("games", genres, parentPlatforms, ordering, search, page, pageSize);
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

        public async Task<CollectionResult<Genre>> GetGenresAsync(int page = 1, int pageSize = 20)
        {
            var cacheKey = BuildCacheKey("genres", null, null, null, null, page, pageSize);
            var cachedResponse = this.genresCache.Get(cacheKey);

            if (cachedResponse != null)
            {
                this.logger.LogInformation($"Cache hit for request URI: {cacheKey}");
                return cachedResponse;
            }

            // fallback to call base service
            var results = await this.rawgApi.GetGenresAsync();

            this.genresCache.Set(cacheKey, results, TimeSpan.FromDays(7));
            this.logger.LogInformation($"Cache miss for request URI: {cacheKey}");

            return results;
        }

        private string BuildCacheKey(string resourceCollection, string? genres, string? parentPlatforms, string? ordering, string? search, int page, int pageSize)
        {
            var cacheKeyBuilder = new StringBuilder($"{resourceCollection}?page={page}&page_size={pageSize}");

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

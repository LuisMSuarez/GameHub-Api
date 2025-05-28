namespace GameHubApi.Providers
{
    using GameHubApi.Contracts;
    using System.Text;

    public class CachedRawgApi : IRawgApi
    {
        private readonly ILruCache<string, CollectionResult<Game>> gamesCollectionCache;
        private readonly ILruCache<string, CollectionResult<Genre>> genresCollectionCache;
        private readonly ILruCache<string, Game> gameDetailsCache;
        private readonly ILogger<CachedRawgApi> logger;
        private readonly IRawgApi rawgApi;
        private const string ResourceKey = "resource";

        public CachedRawgApi(
            ILruCache<string, CollectionResult<Game>> gamesCollectionCache,
            ILruCache<string, CollectionResult<Genre>> genresCollectionCache,
            ILruCache<string, Game> gameDetailsCache,
            ILogger<CachedRawgApi> logger, 
            Func<string, IRawgApi> rawgApiFactory)
        {
            this.gamesCollectionCache = gamesCollectionCache ?? throw new ArgumentNullException(nameof(gamesCollectionCache));
            this.genresCollectionCache = genresCollectionCache ?? throw new ArgumentNullException(nameof(genresCollectionCache));
            this.gameDetailsCache = gameDetailsCache ?? throw new ArgumentNullException(nameof(gameDetailsCache));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (rawgApiFactory == null)
            {
                throw new ArgumentNullException(nameof(rawgApiFactory));
            }

            this.rawgApi = rawgApiFactory("Base") ?? throw new ArgumentNullException(nameof(rawgApiFactory));
        }

        public async Task<CollectionResult<Game>> GetGamesAsync(string? genres, string? parentPlatforms, string? ordering, string? search, int page = 1, int pageSize = 20)
        {
           var propDictionary = new Dictionary<string, string?>
           {
               { ResourceKey, "games" },
               { "genres", genres },
               { "parentPlatforms", parentPlatforms },
               { "ordering", ordering },
               { "search", search },
               { "page", page.ToString() },
               { "pageSize", pageSize.ToString() }
           };

            var cacheKey = BuildCacheKey(propDictionary);
            var cachedResponse = this.gamesCollectionCache.Get(cacheKey);

            if (cachedResponse != null)
            {
                this.logger.LogInformation($"Cache hit for request URI: {cacheKey}");
                return cachedResponse;
            }

            // fallback to call base service  
            var results = await this.rawgApi.GetGamesAsync(genres, parentPlatforms, ordering, search, page, pageSize);

            this.gamesCollectionCache.Set(cacheKey, results, TimeSpan.FromDays(7));
            this.logger.LogInformation($"Cache miss for request URI: {cacheKey}");

            return results;
        }

        public async Task<CollectionResult<Genre>> GetGenresAsync(int page = 1, int pageSize = 20)
        {
            var propDictionary = new Dictionary<string, string?>
            {
                { ResourceKey, "genres" },
                { "page", page.ToString() },
                { "pageSize", pageSize.ToString() }
            };
            var cacheKey = BuildCacheKey(propDictionary);
            var cachedResponse = this.genresCollectionCache.Get(cacheKey);

            if (cachedResponse != null)
            {
                this.logger.LogInformation($"Cache hit for request URI: {cacheKey}");
                return cachedResponse;
            }

            // fallback to call base service
            var results = await this.rawgApi.GetGenresAsync(page, pageSize);

            this.genresCollectionCache.Set(cacheKey, results, TimeSpan.FromDays(7));
            this.logger.LogInformation($"Cache miss for request URI: {cacheKey}");

            return results;
        }

        public async Task<Game> GetGameAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                throw new ArgumentException("Slug cannot be null or empty.", nameof(slug));
            }

            var propDictionary = new Dictionary<string, string?>
            {
                { ResourceKey, "game" },
                { "slug", slug }
            };
            var cacheKey = BuildCacheKey(propDictionary);
            var cachedResponse = this.gameDetailsCache.Get(cacheKey);

            if (cachedResponse != null)
            {
                this.logger.LogInformation($"Cache hit for request URI: {cacheKey}");
                return cachedResponse;
            }

            // fallback to call base service
            var results = await this.rawgApi.GetGameAsync(slug);

            this.gameDetailsCache.Set(cacheKey, results, TimeSpan.FromDays(7));
            this.logger.LogInformation($"Cache miss for request URI: {cacheKey}");

            return results;

        }

        private static string BuildCacheKey(Dictionary<string, string?> parameters)
        {
            var cacheKeyBuilder = new StringBuilder();
            if (!parameters.ContainsKey(ResourceKey))
            {
                throw new ArgumentException("Cache key must contain name of resource collection");
            }

            foreach (var parameter in parameters)
            {
                if (!string.IsNullOrWhiteSpace(parameter.Value))
                {
                    cacheKeyBuilder.Append($"{parameter.Key}={Uri.EscapeDataString(parameter.Value)};");
                }
            }

            return cacheKeyBuilder.ToString();
        }
    }
}

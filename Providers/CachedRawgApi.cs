namespace GameHubApi.Providers
{
    using GameHubApi.Contracts;
    using System.Text;

    public class CachedRawgApi : IRawgApi
    {
        private readonly ILruCache<string, CollectionResult<Game>> gamesCollectionCache;
        private readonly ILruCache<string, CollectionResult<Genre>> genresCollectionCache;
        private readonly ILruCache<string, Game> gameDetailsCache;
        private readonly ILruCache<string, CollectionResult<Movie>> movieCollectionCache;
        private readonly ILruCache<string, CollectionResult<Screenshot>> screenshotCollectionCache;
        private readonly ILogger<CachedRawgApi> logger;
        private readonly IRawgApi rawgApi;

        public CachedRawgApi(
            ILruCache<string, CollectionResult<Game>> gamesCollectionCache,
            ILruCache<string, CollectionResult<Genre>> genresCollectionCache,
            ILruCache<string, Game> gameDetailsCache,
            ILruCache<string, CollectionResult<Movie>> movieCollectionCache,
            ILruCache<string, CollectionResult<Screenshot>> screenshotCollectionCache,
            ILogger<CachedRawgApi> logger, 
            Func<string, IRawgApi> rawgApiFactory)
        {
            this.gamesCollectionCache = gamesCollectionCache ?? throw new ArgumentNullException(nameof(gamesCollectionCache));
            this.genresCollectionCache = genresCollectionCache ?? throw new ArgumentNullException(nameof(genresCollectionCache));
            this.gameDetailsCache = gameDetailsCache ?? throw new ArgumentNullException(nameof(gameDetailsCache));
            this.movieCollectionCache = movieCollectionCache ?? throw new ArgumentNullException(nameof(movieCollectionCache));
            this.screenshotCollectionCache = screenshotCollectionCache ?? throw new ArgumentNullException(nameof(screenshotCollectionCache));

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

        public async Task<Game> GetGameAsync(string gameId)
        {
            if (string.IsNullOrWhiteSpace(gameId))
            {
                throw new ArgumentException("Id cannot be null or empty.", nameof(gameId));
            }

            var propDictionary = new Dictionary<string, string?>
            {
                { "id", gameId }
            };
            var cacheKey = BuildCacheKey(propDictionary);
            var cachedResponse = this.gameDetailsCache.Get(cacheKey);

            if (cachedResponse != null)
            {
                this.logger.LogInformation($"Cache hit for request URI: {cacheKey}");
                return cachedResponse;
            }

            // fallback to call base service
            var results = await this.rawgApi.GetGameAsync(gameId);

            this.gameDetailsCache.Set(cacheKey, results, TimeSpan.FromDays(7));
            this.logger.LogInformation($"Cache miss for request URI: {cacheKey}");

            return results;
        }

        public async Task<CollectionResult<Movie>> GetMovies(string gameId)
        {
            if (string.IsNullOrWhiteSpace(gameId))
            {
                throw new ArgumentException("Id cannot be null or empty.", nameof(gameId));
            }

            var propDictionary = new Dictionary<string, string?>
            {
                { "id", gameId }
            };
            var cacheKey = BuildCacheKey(propDictionary);
            var cachedResponse = this.movieCollectionCache.Get(cacheKey);

            if (cachedResponse != null)
            {
                this.logger.LogInformation($"Cache hit for request URI: {cacheKey}");
                return cachedResponse;
            }

            // fallback to call base service
            var results = await this.rawgApi.GetMovies(gameId);

            this.movieCollectionCache.Set(cacheKey, results, TimeSpan.FromDays(7));
            this.logger.LogInformation($"Cache miss for request URI: {cacheKey}");

            return results;
        }

        public async Task<CollectionResult<Screenshot>> GetScreenshots(string gameId)
        {
            if (string.IsNullOrWhiteSpace(gameId))
            {
                throw new ArgumentException("Id cannot be null or empty.", nameof(gameId));
            }

            var propDictionary = new Dictionary<string, string?>
            {
                { "id", gameId }
            };
            var cacheKey = BuildCacheKey(propDictionary);
            var cachedResponse = this.screenshotCollectionCache.Get(cacheKey);

            if (cachedResponse != null)
            {
                this.logger.LogInformation($"Cache hit for request URI: {cacheKey}");
                return cachedResponse;
            }

            // fallback to call base service
            var results = await this.rawgApi.GetScreenshots(gameId);

            this.screenshotCollectionCache.Set(cacheKey, results, TimeSpan.FromDays(7));
            this.logger.LogInformation($"Cache miss for request URI: {cacheKey}");

            return results;
        }

        private static string BuildCacheKey(Dictionary<string, string?> parameters)
        {
            var cacheKeyBuilder = new StringBuilder();
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

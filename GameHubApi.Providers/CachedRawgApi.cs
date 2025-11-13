namespace GameHubApi.Providers
{
    using GameHubApi.Contracts;
    using Microsoft.Extensions.Logging;
    using System.Text;

    /// <summary>
    /// Provides a cached wrapper around the <see cref="IRawgApi"/> implementation to reduce redundant API calls.
    /// </summary>
    public class CachedRawgApi : IRawgApi
    {
        private readonly ILruCache<string, CollectionResult<Game>> gamesCollectionCache;
        private readonly ILruCache<string, CollectionResult<Genre>> genresCollectionCache;
        private readonly ILruCache<string, Game> gameDetailsCache;
        private readonly ILruCache<string, CollectionResult<Movie>> movieCollectionCache;
        private readonly ILruCache<string, CollectionResult<Screenshot>> screenshotCollectionCache;
        private readonly ILogger<CachedRawgApi> logger;
        private readonly IRawgApi rawgApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedRawgApi"/> class.
        /// </summary>
        /// <param name="gamesCollectionCache">Cache for game collections.</param>
        /// <param name="genresCollectionCache">Cache for genre collections.</param>
        /// <param name="gameDetailsCache">Cache for individual game details.</param>
        /// <param name="movieCollectionCache">Cache for game-related movie trailers.</param>
        /// <param name="screenshotCollectionCache">Cache for game screenshots.</param>
        /// <param name="logger">Logger for diagnostics.</param>
        /// <param name="rawgApiFactory">Factory to create the base <see cref="IRawgApi"/> instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when any required dependency is null.</exception>
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

        /// <inheritdoc />
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

            var results = await this.rawgApi.GetGamesAsync(genres, parentPlatforms, ordering, search, page, pageSize);
            this.gamesCollectionCache.Set(cacheKey, results, TimeSpan.FromDays(7));
            this.logger.LogInformation($"Cache miss for request URI: {cacheKey}");

            return results;
        }

        /// <inheritdoc />
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

            var results = await this.rawgApi.GetGenresAsync(page, pageSize);
            this.genresCollectionCache.Set(cacheKey, results, TimeSpan.FromDays(7));
            this.logger.LogInformation($"Cache miss for request URI: {cacheKey}");

            return results;
        }

        /// <inheritdoc />
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

            var results = await this.rawgApi.GetGameAsync(gameId);
            this.gameDetailsCache.Set(cacheKey, results, TimeSpan.FromDays(7));
            this.logger.LogInformation($"Cache miss for request URI: {cacheKey}");

            return results;
        }

        /// <inheritdoc />
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

            var results = await this.rawgApi.GetMovies(gameId);
            this.movieCollectionCache.Set(cacheKey, results, TimeSpan.FromDays(7));
            this.logger.LogInformation($"Cache miss for request URI: {cacheKey}");

            return results;
        }

        /// <inheritdoc />
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

            var results = await this.rawgApi.GetScreenshots(gameId);
            this.screenshotCollectionCache.Set(cacheKey, results, TimeSpan.FromDays(7));
            this.logger.LogInformation($"Cache miss for request URI: {cacheKey}");

            return results;
        }

        /// <summary>
        /// Builds a unique cache key from a dictionary of query parameters.
        /// </summary>
        /// <param name="parameters">Dictionary of query parameters.</param>
        /// <returns>A string representing the cache key.</returns>
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

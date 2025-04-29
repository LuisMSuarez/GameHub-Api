namespace GameHubApi.Providers
{
    using GameHubApi.Contracts;
    using Microsoft.Extensions.Logging;
    using System.Text;
    using System.Text.Json;
    public class RawgApi : IRawgApi
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "https://api.rawg.io/api";
        private const string SecretName = "RawgApiKey";
        private readonly string apiKey;
        private readonly ILruCache<string, CollectionResult<Game>> gamesCache;
        private readonly ILogger<RawgApi> logger;


        public RawgApi(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILruCache<string, CollectionResult<Game>> gamesCache,
            ILogger<RawgApi> logger)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (httpClientFactory == null)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            var apiKeyValue = configuration[SecretName];
            this.apiKey = apiKeyValue ?? throw new ArgumentNullException(nameof(apiKeyValue));
            this.httpClient = httpClientFactory.CreateClient();
            this.logger = logger;

            // note: cache is optional, we don't need to throw if it's not provided
            this.gamesCache = gamesCache;
        }

        public async Task<CollectionResult<Game>> GetGamesAsync(string? genres, string? parentPlatforms, string? ordering, string? search, int page = 1, int pageSize = 20)
        {
            var urlBuilder = new StringBuilder($"{BaseUrl}/games?key={apiKey}&page={page}&page_size={pageSize}");

            if (!string.IsNullOrWhiteSpace(genres))
            {
                urlBuilder.Append($"&genres={Uri.EscapeDataString(genres)}");
            }
            if (!string.IsNullOrWhiteSpace(parentPlatforms))
            {
                urlBuilder.Append($"&parent_platforms={Uri.EscapeDataString(parentPlatforms)}");
            }
            if (!string.IsNullOrWhiteSpace(ordering))
            {
                urlBuilder.Append($"&ordering={Uri.EscapeDataString(ordering)}");
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                urlBuilder.Append($"&search={Uri.EscapeDataString(search)}");
            }

            var requestUri = urlBuilder.ToString();
            var cachedResponse = this.gamesCache?.Get(requestUri);

            if (cachedResponse != null)
            {
                this.logger.LogInformation($"Cache hit for request URI: {requestUri}");
                return cachedResponse;
            }

            // fallback to making the request
            var response = await httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CollectionResult<Game>>(content);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to deserialize the response content.");
            }

            if (this.gamesCache != null)
            {
                this.logger.LogInformation($"Cache miss for request URI: {requestUri}");
                this.gamesCache.Set(requestUri, result, TimeSpan.FromDays(7));
            }

            return result;
        }
    }
}

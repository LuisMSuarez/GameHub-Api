namespace GameHubApi.Providers
{
    using GameHubApi.Contracts;
    using System.Text;
    using System.Text.Json;
    public class RawgApi : IRawgApi
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "https://api.rawg.io/api";
        private const string SecretName = "RawgApiKey";
        private readonly string apiKey;

        public RawgApi(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (httpClientFactory == null)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }

            var apiKeyValue = configuration[SecretName];
            this.apiKey = apiKeyValue ?? throw new ArgumentNullException(nameof(apiKeyValue));
            this.httpClient = httpClientFactory.CreateClient();
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
            var response = await httpClient.GetAsync(urlBuilder.ToString());

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CollectionResult<Game>>(content);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to deserialize the response content.");
            }

            return result;
        }
    }
}

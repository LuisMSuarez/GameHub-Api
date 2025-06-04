namespace GameHubApi.Providers
{
    using GameHubApi.Contracts;
    using System;
    using System.Net;
    using System.Text;
    using System.Text.Json;
    using System.Web;
    public class RawgApi : IRawgApi
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "https://api.rawg.io/api";
        private const string SecretName = "RawgApiKey";
        private readonly string apiKey;
        private readonly IHttpContextAccessor httpContextAccessor; // Add IHttpContextAccessor

        public RawgApi(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            if (httpClientFactory == null)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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
            var result = await GetAsync<CollectionResult<Game>>(urlBuilder.ToString());
            this.UpdatePaginationLinks(result);
            return result;
        }
        public async Task<CollectionResult<Genre>> GetGenresAsync(int page = 1, int pageSize = 20)
        {
            var url = $"{BaseUrl}/genres?key={apiKey}&page={page}&page_size={pageSize}";
            var result = await GetAsync<CollectionResult<Genre>>(url);
            this.UpdatePaginationLinks(result);
            return result;
        }

        public Task<Game> GetGameAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Id cannot be null or empty.", nameof(id));
            }
            var url = $"{BaseUrl}/games/{Uri.EscapeDataString(id)}?key={apiKey}";
            return GetAsync<Game>(url);
        }

        public async Task<CollectionResult<Movie>> GetMovies(string gameId)
        {
            if (string.IsNullOrWhiteSpace(gameId))
            {
                throw new ArgumentException("Id cannot be null or empty.", nameof(gameId));
            }
            var url = $"{BaseUrl}/games/{Uri.EscapeDataString(gameId)}/movies?key={apiKey}";
            var result = await GetAsync<CollectionResult<Movie>>(url);
            this.UpdatePaginationLinks(result);
            return result;
        }
        public async Task<CollectionResult<Screenshot>> GetScreenshots(string gameId)
        {
            if (string.IsNullOrWhiteSpace(gameId))
            {
                throw new ArgumentException("Id cannot be null or empty.", nameof(gameId));
            }
            var url = $"{BaseUrl}/games/{Uri.EscapeDataString(gameId)}/screenshots?key={apiKey}";
            var result = await GetAsync<CollectionResult<Screenshot>>(url);
            this.UpdatePaginationLinks(result);
            return result;
        }

        private async Task<T> GetAsync<T>(string url)
        {
            var response = await httpClient.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                // Handle case where RAWG API returns 404 Not Found
                throw new HttpRequestException("The requested resource was not found.", null, HttpStatusCode.NotFound);
            }
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(content);
            if (result == null)
            {
                throw new InvalidOperationException("Failed to deserialize the response content.");
            }
            return result;
        }

        private void UpdatePaginationLinks<T>(CollectionResult<T>? results)
        {
            if (results == null)
            {
                return;
            }

            if (httpContextAccessor.HttpContext == null)
            {
                throw new InvalidOperationException("HttpContext is not available.");
            }

            // Obtain the hostname from the current HTTP context
            var uriScheme = httpContextAccessor.HttpContext.Request.Scheme;
            var uriHostname = httpContextAccessor.HttpContext.Request.Host.Value;
            var uriPath = httpContextAccessor.HttpContext.Request.Path.Value;

            if (results.Next != null)
            {
                var uri = new Uri(results.Next);
                var query = HttpUtility.ParseQueryString(uri.Query);

                // Rawg API returns the key in the query string, we need to remove it to avoid exposing it
                query.Remove("key");
                results.Next = $"{uriScheme}://{uriHostname}{uriPath}?{query}";
            }
            if (results.Previous != null)
            {
                var uri = new Uri(results.Previous);
                var query = HttpUtility.ParseQueryString(uri.Query);

                // Rawg API returns the key in the query string, we need to remove it to avoid exposing it
                query.Remove("key");
                results.Previous = $"{uriScheme}://{uriHostname}{uriPath}?{query}";
            }
        }
    }
}

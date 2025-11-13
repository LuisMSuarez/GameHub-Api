namespace GameHubApi.Providers
{
    using GameHubApi.Contracts;
    using GameHubApi.Providers.Exceptions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Net;
    using System.Text;
    using System.Text.Json;
    using System.Web;

    /// <summary>
    /// Provides access to the RAWG API for retrieving game-related data such as games, genres, movies, and screenshots.
    /// </summary>
    public class RawgApi : IRawgApi
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "https://api.rawg.io/api";
        private const string SecretName = "RawgApiKey";
        private readonly string apiKey;
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawgApi"/> class.
        /// </summary>
        /// <param name="httpClientFactory">Factory to create <see cref="HttpClient"/> instances.</param>
        /// <param name="configuration">Configuration containing the RAWG API key.</param>
        /// <param name="httpContextAccessor">Accessor for the current HTTP context.</param>
        /// <exception cref="ArgumentNullException">Thrown when any required dependency is null.</exception>
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<CollectionResult<Genre>> GetGenresAsync(int page = 1, int pageSize = 20)
        {
            var url = $"{BaseUrl}/genres?key={apiKey}&page={page}&page_size={pageSize}";
            var result = await GetAsync<CollectionResult<Genre>>(url);
            this.UpdatePaginationLinks(result);
            return result;
        }

        /// <inheritdoc />
        public async Task<Game> GetGameAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Id cannot be null or empty.", nameof(id));
            }

            var url = $"{BaseUrl}/games/{Uri.EscapeDataString(id)}?key={apiKey}";
            var rawgGame = await GetAsync<RawgApiEntities.Game>(url);

            // Handle redirect scenario if RAWG returns a slug pointing to another game
            if (rawgGame.Redirect.HasValue &&
                rawgGame.Redirect.Value &&
                !string.IsNullOrWhiteSpace(rawgGame.Slug))
            {
                return await GetGameAsync(rawgGame.Slug!);
            }

            return rawgGame.ToContract();
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <summary>
        /// Sends a GET request to the specified URL and deserializes the response into the specified type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response into.</typeparam>
        /// <param name="url">The URL to send the request to.</param>
        /// <returns>The deserialized response object.</returns>
        /// <exception cref="ProviderException">Thrown if the response is null, not found, or deserialization fails.</exception>
        private async Task<T> GetAsync<T>(string url)
        {
            var response = await httpClient.GetAsync(url);
            if (response == null)
            {
                throw new ProviderException(
                    ProviderResultCode.DataAccessError,
                    "The response from the RAWG API Get call was null.");
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new ProviderException(
                    ProviderResultCode.NotFound,
                    "The requested resource was not found.");
            }

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(content);

            if (result == null)
            {
                throw new ProviderException(
                    ProviderResultCode.DataAccessError,
                    "Failed to deserialize the response.");
            }

            return result;
        }

        /// <summary>
        /// Updates pagination links in the result to remove sensitive query parameters and match the current request context.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection result.</typeparam>
        /// <param name="results">The collection result to update.</param>
        /// <exception cref="ProviderException">Thrown if the HTTP context is unavailable.</exception>
        private void UpdatePaginationLinks<T>(CollectionResult<T>? results)
        {
            if (results == null)
            {
                return;
            }

            if (httpContextAccessor.HttpContext == null)
            {
                throw new ProviderException(
                    ProviderResultCode.InternalServerError,
                    "HttpContext is not available.");
            }

            var uriScheme = httpContextAccessor.HttpContext.Request.Scheme;
            var uriHostname = httpContextAccessor.HttpContext.Request.Host.Value;
            var uriPath = httpContextAccessor.HttpContext.Request.Path.Value;

            if (results.Next != null)
            {
                var uri = new Uri(results.Next);
                var query = HttpUtility.ParseQueryString(uri.Query);
                query.Remove("key");
                results.Next = $"{uriScheme}://{uriHostname}{uriPath}?{query}";
            }

            if (results.Previous != null)
            {
                var uri = new Uri(results.Previous);
                var query = HttpUtility.ParseQueryString(uri.Query);
                query.Remove("key");
                results.Previous = $"{uriScheme}://{uriHostname}{uriPath}?{query}";
            }
        }
    }
}

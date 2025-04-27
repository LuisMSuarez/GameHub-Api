namespace GameHubApi.Providers
{
    using GameHubApi.Contracts;
    using System.Text.Json;
    public class RawgApi
    {
        private readonly HttpClient httpClient;
        private const string BaseUrl = "https://api.rawg.io/api";
        private readonly string apiKey;

        public RawgApi(HttpClient httpClient, string apiKey)
        {
            this.httpClient = httpClient;
            this.apiKey = apiKey;
        }

        public async Task<CollectionResult<Game>> GetGamesAsync(int page = 1, int pageSize = 20)
        {
            var url = $"{BaseUrl}/games?key={apiKey}&page={page}&page_size={pageSize}";
            var response = await httpClient.GetAsync(url);

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

using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using GameHubApi.Contracts;
using GameHubApi.Controllers;
using GameHubApi.Providers;

namespace GameHubApi.Services
{
    public class GamesService : IGamesService
    {
        private readonly RawgApi gamesProvider;
        private const string SecretName = "RawgApiKey";
        private readonly ILogger<GamesService> logger;
        public GamesService(ILogger<GamesService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            var apiKey = configuration[SecretName];
            if (string.IsNullOrEmpty(apiKey))
            {
                this.logger.LogError("API key is not set in the configuration.");
                throw new InvalidOperationException("API key is not set in the configuration.");
            }

            gamesProvider = new RawgApi(new HttpClient(), apiKey);
        }
        public async Task<CollectionResult<Game>> GetGamesAsync()
        {
            return await gamesProvider.GetGamesAsync();
        }
    }
}

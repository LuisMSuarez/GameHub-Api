using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using GameHubApi.Contracts;
using GameHubApi.Providers;

namespace GameHubApi.Services
{
    public class GamesService : IGamesService
    {
        private readonly RawgApi gamesProvider;
        private const string KeyVaultUrl = "https://kv-gamers-hub.vault.azure.net/";
        private const string SecretName = "RawgApiKey";
        public GamesService()
        {
            // Fetch the API key from Azure Key Vault
            var client = new SecretClient(new Uri(KeyVaultUrl), new DefaultAzureCredential());
            var secret = client.GetSecret(SecretName);
            var apiKey = secret.Value.Value;

            gamesProvider = new RawgApi(new HttpClient(), apiKey);
        }
        public async Task<CollectionResult<Game>> GetGamesAsync()
        {
            return await gamesProvider.GetGamesAsync();
        }
    }
}

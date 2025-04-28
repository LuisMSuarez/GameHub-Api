using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using GameHubApi.Contracts;
using GameHubApi.Controllers;
using GameHubApi.Providers;

namespace GameHubApi.Services
{
    public class GamesService : IGamesService
    {
        private readonly IRawgApi rawgApi;
        private readonly ILogger<GamesService> logger;
        public GamesService(ILogger<GamesService> logger, IRawgApi rawgApi)
        {
            this.logger =  logger ?? throw new ArgumentNullException(nameof(logger));
            this.rawgApi = rawgApi ?? throw new ArgumentNullException(nameof(rawgApi));
        }
        public async Task<CollectionResult<Game>> GetGamesAsync(string? genres, string? parentPlatforms, string? ordering, string? search)
        {
            return await rawgApi.GetGamesAsync(genres, parentPlatforms, ordering, search);
        }
    }
}

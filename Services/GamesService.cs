using GameHubApi.Contracts;
using GameHubApi.Providers;

namespace GameHubApi.Services
{
    public class GamesService : IGamesService
    {
        private readonly RawgApi gamesProvider;

        public GamesService()
        {
            gamesProvider = new RawgApi(new HttpClient(), "fetch from keyvault");
        }
        public async Task<CollectionResult<Game>> GetGamesAsync()
        {
            return await gamesProvider.GetGamesAsync();
        }
    }
}

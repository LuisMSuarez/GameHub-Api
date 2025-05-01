namespace GameHubApi.Services
{
    using GameHubApi.Contracts;
    using GameHubApi.Providers;

    public class GamesService : IGamesService
    {
        private readonly IRawgApi rawgApi;
        public GamesService(IRawgApi rawgApi)
        {
            this.rawgApi = rawgApi ?? throw new ArgumentNullException(nameof(rawgApi));
        }
        public async Task<CollectionResult<Game>> GetGamesAsync(string? genres, string? parentPlatforms, string? ordering, string? search)
        {
            return await rawgApi.GetGamesAsync(genres, parentPlatforms, ordering, search);
        }
    }
}

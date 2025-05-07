namespace GameHubApi.Services
{
    using GameHubApi.Contracts;
    using GameHubApi.Providers;

    public class GamesService : IGamesService
    {
        private readonly IRawgApi rawgApi;
        private readonly IGameFilter gameFilter;
        public GamesService(IRawgApi rawgApi, IGameFilter gameFilter)
        {
            this.rawgApi = rawgApi ?? throw new ArgumentNullException(nameof(rawgApi));
            this.gameFilter = gameFilter ?? throw new ArgumentNullException(nameof(gameFilter));
        }
        public async Task<CollectionResult<Game>> GetGamesAsync(string? genres, string? parentPlatforms, string? ordering, string? search, int page, int pageSize)
        {
            var getGamesResult = await rawgApi.GetGamesAsync(genres, parentPlatforms, ordering, search, page, pageSize);
            var filteredGames = getGamesResult.Results.Where(g => this.gameFilter.Filter(g) == FilterResult.Passed);
            return new CollectionResult<Game>
            {
                Count = getGamesResult.Count,
                Next = getGamesResult.Next,
                Previous = getGamesResult.Previous,
                Results = filteredGames.ToList()
            };
        }
    }
}

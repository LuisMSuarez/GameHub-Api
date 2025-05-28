namespace GameHubApi.Services
{
    using GameHubApi.Contracts;
    using GameHubApi.Providers;
    using System.Net;

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
            var getGamesResult = await this.rawgApi.GetGamesAsync(genres, parentPlatforms, ordering, search, page, pageSize);
            var filteredGames = getGamesResult.Results.Where(g => this.gameFilter.Filter(g) == FilterResult.Passed);
            return new CollectionResult<Game>
            {
                Count = getGamesResult.Count,
                Next = getGamesResult.Next,
                Previous = getGamesResult.Previous,
                Results = filteredGames.ToList()
            };
        }

        public async Task<Game> GetGameAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                throw new ArgumentException("Slug cannot be null or empty.", nameof(slug));
            }

            var game = await this.rawgApi.GetGameAsync(slug);
            if (game != null)
            {
                if (this.gameFilter.Filter(game) == FilterResult.Passed)
                {
                    return game;
                }
                else
                {
                    throw new HttpRequestException("The requested game does not pass the filter criteria.", null, HttpStatusCode.Forbidden);
                }
            }

            throw new HttpRequestException("Game not found.", null, HttpStatusCode.NotFound);

        }
    }
}

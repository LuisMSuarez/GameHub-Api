namespace GameHubApi.Services
{
    using GameHubApi.Contracts;
    using GameHubApi.Providers;
    using System.Net;

    public class GamesService : IGamesService
    {
        private readonly IRawgApi rawgApi;
        private readonly IGameFilter gameFilter;
        private readonly ITranslator translator;
        public GamesService(IRawgApi rawgApi, IGameFilter gameFilter, ITranslator translator)
        {
            this.rawgApi = rawgApi ?? throw new ArgumentNullException(nameof(rawgApi));
            this.gameFilter = gameFilter ?? throw new ArgumentNullException(nameof(gameFilter));
            this.translator = translator ?? throw new ArgumentNullException(nameof(translator));
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

        public async Task<Game> GetGameAsync(string gameId, string? language)
        {
            if (string.IsNullOrWhiteSpace(gameId))
            {
                throw new ArgumentException("Id cannot be null or empty.", nameof(gameId));
            }

            var game = await this.rawgApi.GetGameAsync(gameId);
            if (game != null)
            {
                if (this.gameFilter.Filter(game) == FilterResult.Passed)
                {
                    if (!string.IsNullOrWhiteSpace(language) && !string.IsNullOrWhiteSpace(game.Description))
                    {
                        var translation = await this.translator.Translate(game.Description, null, language);
                        game.Description = translation;
                    }
                    return game;
                }
                else
                {
                    throw new HttpRequestException("The requested game does not pass the filter criteria.", null, HttpStatusCode.Forbidden);
                }
            }

            throw new HttpRequestException("Game not found.", null, HttpStatusCode.NotFound);
        }

        public async Task<CollectionResult<Movie>> GetMovies(string gameId)
        {
            return await this.rawgApi.GetMovies(gameId);
        }

        public async Task<CollectionResult<Screenshot>> GetScreenshots(string gameId)
        {
            return await this.rawgApi.GetScreenshots(gameId);
        }
    }
}

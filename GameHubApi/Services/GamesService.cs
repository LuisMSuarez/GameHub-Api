namespace GameHubApi.Services
{
    using GameHubApi.Contracts;
    using GameHubApi.Providers;
    using GameHubApi.Providers.Exceptions;
    using GameHubApi.Services.Exceptions;

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

            Game game;
            try
            {
                game = await this.rawgApi.GetGameAsync(gameId);
            }
            catch (ProviderException ex) when (ex.ResultCode == ProviderResultCode.NotFound)
            {
                throw new ServiceException(
                    ServiceResultCode.NotFound,
                    "Game not found.", ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException(
                    ServiceResultCode.InternalServerError,
                    "An error occurred while fetching the game details.", ex);
            }

            if (game == null)
            {
                throw new ServiceException(
                    ServiceResultCode.NotFound,
                    "Game not found.");
            }

            if (this.gameFilter.Filter(game) != FilterResult.Passed)
            {
                throw new ServiceException(
                    ServiceResultCode.Forbidden,
                    "The requested game does not pass the filter criteria.");
            }

            if (!string.IsNullOrWhiteSpace(language) && !string.IsNullOrWhiteSpace(game.Description))
            {
                var translation = await this.translator.Translate(game.Description, null, language);
                
                // we need to clone the game object to avoid modifying the original which is in the cache, given that it is a reference object
                var clonedGame = game.Clone() as Game ?? throw new InvalidOperationException("Failed to clone the game object.");
                clonedGame.Description = translation;
                return clonedGame;
            }
            return game;
        }

        public async Task<CollectionResult<Movie>> GetMovies(string gameId)
        {
            try
            {
                return await this.rawgApi.GetMovies(gameId);
            }
            catch (ProviderException ex) when (ex.ResultCode == ProviderResultCode.NotFound)
            {
                throw new ServiceException(
                    ServiceResultCode.NotFound,
                    "Movies not found for the specified game.", ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException(
                    ServiceResultCode.InternalServerError,
                    "An error occurred while fetching the movies.", ex);
            }
        }

        public async Task<CollectionResult<Screenshot>> GetScreenshots(string gameId)
        {
            try
            {
                return await this.rawgApi.GetScreenshots(gameId);
            }
            catch (ProviderException ex) when (ex.ResultCode == ProviderResultCode.NotFound)
            {
                throw new ServiceException(
                    ServiceResultCode.NotFound,
                    "Screenshots not found for the specified game.", ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException(
                    ServiceResultCode.InternalServerError,
                    "An error occurred while fetching the screenshots.", ex);
            }
        }
    }
}

namespace GameHubApi.Services
{
    using GameHubApi.Contracts;
    using GameHubApi.Providers;
    using GameHubApi.Providers.Contracts;
    using GameHubApi.Providers.Exceptions;
    using GameHubApi.Services.Exceptions;
    using Microsoft.Extensions.Logging;
    using System.Text.Json;

    public class GamesService : IGamesService
    {
        private readonly IRawgApi rawgApi;
        private readonly IGameFilter gameFilter;
        private readonly ITranslator translator;
        private readonly ILargeLanguageModel largeLanguageModel;
        private readonly ILogger<GamesService> logger;

        public GamesService(
            IRawgApi rawgApi,
            IGameFilter gameFilter,
            ITranslator translator,
            ILargeLanguageModel largeLanguageModel,
            ILogger<GamesService> logger)
        {
            this.rawgApi = rawgApi ?? throw new ArgumentNullException(nameof(rawgApi));
            this.gameFilter = gameFilter ?? throw new ArgumentNullException(nameof(gameFilter));
            this.translator = translator ?? throw new ArgumentNullException(nameof(translator));
            this.largeLanguageModel = largeLanguageModel ?? throw new ArgumentNullException(nameof(largeLanguageModel));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CollectionResult<Game>> GetGamesAsync(string? genres, string? parentPlatforms, string? ordering, string? search, int page, int pageSize)
        {
            var getGamesResult = await this.rawgApi.GetGamesAsync(genres, parentPlatforms, ordering, search, page, pageSize);
            var filteredGames = await Task.WhenAll(getGamesResult.Results.Select(async g => new
            {
                Game = g,
                FilterResult = await this.gameFilter.FilterAsync(g)
            }));

            return new CollectionResult<Game>
            {
                Count = getGamesResult.Count,
                Next = getGamesResult.Next,
                Previous = getGamesResult.Previous,
                Results = filteredGames.Where(g => g.FilterResult == FilterResult.Passed).Select(g => g.Game).ToList()
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

            if (await this.gameFilter.FilterAsync(game) != FilterResult.Passed)
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

        public async Task<CollectionResult<Game>> GetGameRecommendationsAsync(GameRecommendationsRequest request)
        {
            var result = await this.largeLanguageModel.GenerateResponseAsync(new GenerateResponseQuery
            {
                Instructions = @"You are a game recommendation engine, based on the RAWG api game catalog.
                                 You will recommend 10 games, based on the input provided of a list of games the user likes,
                                 and a list of games the user dislikes. The recommended games will be sorted by relevance to the user's preferences.
                                 where the first item is the most relevant.
                                 Your reply will be in raw JSON format, no markdown or code blocks, with an array of objects with this schema.
                                 [{ ""name"": ""Grand Theft Auto V"", ""slug"": ""grand-theft-auto-v"" }]",
                Query = "List of games the user likes:\n" + JsonSerializer.Serialize(request.LikedGames) +
                        "\nList of games the user dislikes:\n" + JsonSerializer.Serialize(request.DislikedGames)
            });
            
            try
            {   // Try to parse the response to ensure it's valid JSON
                JsonDocument.Parse(result.Message);
            }
            catch (JsonException ex)
            {
                this.logger.LogError(ex, "Failed to parse AI recommendation response as JSON.");
                this.logger.LogError("AI Recommendation Response: {Response}", result.Message);
                throw new ServiceException(
                    ServiceResultCode.InternalServerError,
                    "Failed to parse AI recommendation response as JSON.", ex);
            }
            
            var recommendedGames = JsonSerializer.Deserialize<List<RecommendedGame>>(result.Message);
            
            // Hydrate the recommended games with full game details
            // We requested 10 recommendations, but we may get less after filtering out not found or forbidden games,
            // so we take up to 5 valid games to return
            var hydratedRecommendations = recommendedGames != null
                    ? (await Task.WhenAll(recommendedGames.Select(async rg =>
                    {
                        try
                        {
                            var game = await this.GetGameAsync(rg.Slug, null);
                            return game;
                        }
                        catch (ServiceException ex) when (
                            ex.ResultCode == ServiceResultCode.NotFound ||
                            ex.ResultCode == ServiceResultCode.Forbidden)
                        {
                            return null;
                        }
                    }))).Where(g => g != null).Take(5).ToList()!
                    : new List<Game>();

            return new CollectionResult<Game>
            {
                Count = hydratedRecommendations?.Count ?? 0,
                Next = null,
                Previous = null,
                Results = hydratedRecommendations!
            };
        }
    }
}

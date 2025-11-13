namespace GameHubApi.Services
{
    using GameHubApi.Contracts;

    /// <summary>
    /// Defines a contract for retrieving and managing game-related data and recommendations.
    /// </summary>
    public interface IGamesService
    {
        /// <summary>
        /// Retrieves a paginated list of games with optional filtering and search parameters.
        /// </summary>
        /// <param name="genres">Comma-separated genre identifiers to filter games.</param>
        /// <param name="parentPlatforms">Comma-separated platform identifiers to filter games.</param>
        /// <param name="ordering">Ordering criteria (e.g., rating, released).</param>
        /// <param name="search">Search term to filter games by name.</param>
        /// <param name="page">Page number for pagination.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <returns>A collection of games matching the specified criteria.</returns>
        Task<CollectionResult<Game>> GetGamesAsync(string? genres, string? parentPlatforms, string? ordering, string? search, int page, int pageSize);

        /// <summary>
        /// Retrieves detailed information about a specific game, optionally translating its description.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        /// <param name="language">Optional target language code for translating the game description.</param>
        /// <returns>The game details, potentially with a translated description.</returns>
        Task<Game> GetGameAsync(string gameId, string? language);

        /// <summary>
        /// Retrieves a collection of movie trailers or cinematic content related to a specific game.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        /// <returns>A collection of movies associated with the game.</returns>
        Task<CollectionResult<Movie>> GetMovies(string gameId);

        /// <summary>
        /// Retrieves a collection of screenshots for a specific game.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        /// <returns>A collection of screenshots associated with the game.</returns>
        Task<CollectionResult<Screenshot>> GetScreenshots(string gameId);

        /// <summary>
        /// Generates personalized game recommendations based on user preferences using AI.
        /// </summary>
        /// <param name="request">The recommendation request containing liked and disliked games.</param>
        /// <returns>A collection of recommended games.</returns>
        Task<CollectionResult<Game>> GetGameRecommendationsAsync(GameRecommendationsRequest request);
    }
}

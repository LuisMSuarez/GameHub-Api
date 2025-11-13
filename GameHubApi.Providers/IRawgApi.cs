namespace GameHubApi.Providers
{
    using GameHubApi.Contracts;

    /// <summary>
    /// Defines a contract for accessing game-related data from the RAWG API.
    /// </summary>
    public interface IRawgApi
    {
        /// <summary>
        /// Retrieves a paginated collection of games based on optional filters.
        /// </summary>
        /// <param name="genres">Comma-separated genre identifiers to filter games.</param>
        /// <param name="parentPlatforms">Comma-separated platform identifiers to filter games.</param>
        /// <param name="ordering">Ordering criteria (e.g., rating, released).</param>
        /// <param name="search">Search term to filter games by name.</param>
        /// <param name="page">Page number for pagination (default is 1).</param>
        /// <param name="pageSize">Number of items per page (default is 20).</param>
        /// <returns>A collection of games matching the specified criteria.</returns>
        Task<CollectionResult<Game>> GetGamesAsync(string? genres, string? parentPlatforms, string? ordering, string? search, int page = 1, int pageSize = 20);

        /// <summary>
        /// Retrieves a paginated collection of game genres.
        /// </summary>
        /// <param name="page">Page number for pagination (default is 1).</param>
        /// <param name="pageSize">Number of items per page (default is 20).</param>
        /// <returns>A collection of available game genres.</returns>
        Task<CollectionResult<Genre>> GetGenresAsync(int page = 1, int pageSize = 20);

        /// <summary>
        /// Retrieves detailed information about a specific game.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        /// <returns>The game details.</returns>
        Task<Game> GetGameAsync(string gameId);

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
    }
}

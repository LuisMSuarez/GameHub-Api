namespace GameHubApi.Services
{
    using GameHubApi.Contracts;

    /// <summary>
    /// Defines a contract for retrieving genre-related data from the game catalog.
    /// </summary>
    public interface IGenresService
    {
        /// <summary>
        /// Retrieves a paginated list of game genres.
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of genres per page.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a collection of genres.
        /// </returns>
        Task<CollectionResult<Genre>> GetGenresAsync(int page, int pageSize);
    }
}

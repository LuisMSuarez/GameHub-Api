namespace GameHubApi.Services
{
    using GameHubApi.Contracts;
    using GameHubApi.Providers;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides access to genre-related data using the RAWG API.
    /// </summary>
    public class GenresService : IGenresService
    {
        private readonly IRawgApi rawgApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenresService"/> class.
        /// </summary>
        /// <param name="rawgApi">The RAWG API provider used to fetch genre data.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rawgApi"/> is null.</exception>
        public GenresService(IRawgApi rawgApi)
        {
            this.rawgApi = rawgApi ?? throw new ArgumentNullException(nameof(rawgApi));
        }

        /// <summary>
        /// Retrieves a paginated list of game genres.
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of genres per page.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of genres.</returns>
        public Task<CollectionResult<Genre>> GetGenresAsync(int page, int pageSize)
        {
            return this.rawgApi.GetGenresAsync(page, pageSize);
        }
    }
}

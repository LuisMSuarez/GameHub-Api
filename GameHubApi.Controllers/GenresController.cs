namespace GameHubApi.Controllers
{
    using GameHubApi.Contracts;
    using GameHubApi.Services;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// API controller responsible for handling genre-related endpoints.
    /// Provides access to paginated genre data used for filtering and categorization.
    /// </summary>
    [Route("v1/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenresService genresService;
        private readonly ILogger<GenresController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenresController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for diagnostics and telemetry.</param>
        /// <param name="genresService">Service responsible for retrieving genre data.</param>
        public GenresController(ILogger<GenresController> logger, IGenresService genresService)
        {
            this.genresService = genresService ?? throw new ArgumentNullException(nameof(genresService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves a paginated list of game genres.
        /// </summary>
        /// <param name="page">Page number to retrieve (default is 1).</param>
        /// <param name="pageSize">Number of items per page (default is 20).</param>
        /// <returns>A collection of genres wrapped in a pagination container.</returns>
        [HttpGet(Name = "genres")]
        public Task<CollectionResult<Genre>> GetGenresAsync(
            [FromQuery(Name = "page")] int page = 1,
            [FromQuery(Name = "page_size")] int pageSize = 20)
        {
            this.logger.LogInformation("GetGenresAsync called to fetch genres.");

            try
            {
                // Delegate to the service layer to retrieve genre data
                return this.genresService.GetGenresAsync(page, pageSize);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while fetching genres in GetGenresAsync.");
                throw; // Allow global exception middleware to handle the error
            }
        }
    }
}

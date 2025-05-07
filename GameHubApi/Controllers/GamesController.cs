namespace GameHubApi.Controllers
{
    using GameHubApi.Contracts;
    using GameHubApi.Services;
    using Microsoft.AspNetCore.Mvc;

    [Route("v1/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGamesService gamesService;
        private readonly ILogger<GamesController> logger;

        public GamesController(ILogger<GamesController> logger, IGamesService gamesService, IConfiguration configuration)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.gamesService = gamesService ?? throw new ArgumentNullException(nameof(gamesService));
        }

        [HttpGet(Name = "games")]
        public async Task<CollectionResult<Game>> GetGamesAsync(
            [FromQuery(Name = "genres")] string? genres,
            [FromQuery(Name = "parent_platforms")] string? parentPlatforms,
            [FromQuery(Name = "ordering")] string? ordering,
            [FromQuery(Name = "search")] string? search,
            [FromQuery(Name = "page")] int page = 1,
            [FromQuery(Name = "page_size")] int pageSize = 20
            )
        {
            this.logger.LogInformation("GetGamesAsync called to fetch games.");

            try
            {
                var result = await gamesService.GetGamesAsync(genres, parentPlatforms, ordering, search, page, pageSize);
                this.logger.LogInformation("GetGamesAsync successfully fetched {Count} games.", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while fetching games in GetGamesAsync.");
                throw; // Re-throw the exception to ensure proper error handling
            }
        }
    }
}

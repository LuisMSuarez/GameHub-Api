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

        public GamesController(ILogger<GamesController> logger, IGamesService gamesService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.gamesService = gamesService ?? throw new ArgumentNullException(nameof(gamesService));
        }

        [HttpGet(Name = "games")]
        public async Task<IActionResult> GetGamesAsync(
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
                return Ok(result);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                this.logger.LogWarning(ex, "The requested resource was not found in GetGamesAsync.");
                return NotFound();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while fetching games in GetGamesAsync.");
                throw; // Re-throw the exception to ensure proper error handling
            }
        }

        // get game by slug
        [HttpGet("{slug}", Name = "game")]
        public async Task<IActionResult> GetGameAsync(string slug)
        {
            this.logger.LogInformation("GetGameAsync called for slug: {Slug}", slug);
            if (string.IsNullOrWhiteSpace(slug))
            {
                this.logger.LogWarning("GetGameAsync called with an empty or null slug.");
                return BadRequest("Slug cannot be null or empty.");
            }
            try
            {
                var game = await gamesService.GetGameAsync(slug);
                if (game == null)
                {
                    this.logger.LogWarning("Game with slug {Slug} not found.", slug);
                    return NotFound();
                }
                this.logger.LogInformation("GetGameAsync successfully fetched game with slug: {Slug}", slug);
                return Ok(game);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                this.logger.LogWarning(ex, "The requested game was not found in GetGameAsync for slug: {Slug}", slug);
                return NotFound();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while fetching the game in GetGameAsync for slug: {Slug}", slug);
                throw; // Re-throw the exception to ensure proper error handling
            }
        }
    }
}
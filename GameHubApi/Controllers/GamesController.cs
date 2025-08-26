namespace GameHubApi.Controllers
{
    using GameHubApi.Services;
    using GameHubApi.Services.Exceptions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

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

        [HttpGet(Name = "GetGames")]
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
            catch (ServiceException ex) when (ex.ResultCode == ServiceResultCode.NotFound)
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

        [HttpGet("{id}", Name = "GetGameDetails")]
        public async Task<IActionResult> GetGameDetailsAsync(string id, [FromQuery] string? language)
        {
            this.logger.LogInformation("GetGameAsync called for id: {id}", id);
            if (string.IsNullOrWhiteSpace(id))
            {
                this.logger.LogWarning("GetGameAsync called with an empty or null id.");
                return BadRequest("Id cannot be null or empty.");
            }
            try
            {
                var game = await gamesService.GetGameAsync(id, language);
                if (game == null)
                {
                    this.logger.LogWarning("Game with id {Id} not found.", id);
                    return NotFound();
                }
                this.logger.LogInformation("GetGameAsync successfully fetched game with id: {Id}", id);
                return Ok(game);
            }
            catch (ServiceException ex) when (ex.ResultCode == ServiceResultCode.NotFound)
            {
                this.logger.LogWarning(ex, "The requested game was not found in GetGameAsync for id: {Id}", id);
                return NotFound();
            }
            catch (ServiceException ex) when (ex.ResultCode == ServiceResultCode.Forbidden)
            {
                this.logger.LogWarning(ex, "The requested game cannot be accessed in GetGameAsync for id {Id}", id);
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while fetching the game in GetGameAsync for id: {Id}", id);
                throw; // Re-throw the exception to ensure proper error handling
            }
        }

        [HttpGet("{id}/movies", Name = "GetGameMovies")]
        public async Task<IActionResult> GetGameMoviesAsync(string id)
        {
            this.logger.LogInformation("GetGameMoviesAsync called for id: {id}", id);
            if (string.IsNullOrWhiteSpace(id))
            {
                this.logger.LogWarning("GetGameMoviesAsync called with an empty or null id.");
                return BadRequest("Id cannot be null or empty.");
            }

            try
            {
                var result = await gamesService.GetMovies(id);
                this.logger.LogInformation("GetGameMoviesAsync successfully fetched {Count} movies.", result.Count);
                return Ok(result);
            }
            catch (ServiceException ex) when (ex.ResultCode == ServiceResultCode.NotFound)
            {
                this.logger.LogWarning(ex, "The requested resource was not found in GetGameMoviesAsync.");
                return NotFound();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while fetching movies in GetGameMoviesAsync.");
                throw; // Re-throw the exception to ensure proper error handling
            }
        }

        [HttpGet("{id}/screenshots", Name = "GetGameScreenshots")]
        public async Task<IActionResult> GetGameScreenshotsAsync(string id)
        {
            this.logger.LogInformation("GetGameScreenshotsAsync called for id: {id}", id);
            if (string.IsNullOrWhiteSpace(id))
            {
                this.logger.LogWarning("GetGameScreenshotsAsync called with an empty or null id.");
                return BadRequest("Id cannot be null or empty.");
            }

            try
            {
                var result = await gamesService.GetScreenshots(id);
                this.logger.LogInformation("GetGameScreenshotsAsync successfully fetched {Count} movies.", result.Count);
                return Ok(result);
            }
            catch (ServiceException ex) when (ex.ResultCode == ServiceResultCode.NotFound)
            {
                this.logger.LogWarning(ex, "The requested resource was not found in GetGameScreenshotsAsync.");
                return NotFound();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while fetching scfreenshots in GetGameScreenshotsAsync.");
                throw; // Re-throw the exception to ensure proper error handling
            }
        }
    }
}
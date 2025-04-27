using GameHubApi.Contracts;
using GameHubApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameHubApi.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGamesService gamesService;
        private readonly ILogger<GamesController> logger;

        public GamesController(ILogger<GamesController> logger, ILogger<GamesService> gamesServiceLogger, IConfiguration configuration)
        {
            this.logger = logger;
            this.gamesService = new GamesService(gamesServiceLogger, configuration);
        }

        [HttpGet(Name = "games")]
        public async Task<CollectionResult<Game>> GetGamesAsync(
            [FromQuery(Name = "genres")] string? genres,
            [FromQuery(Name = "parent_platforms")] string? parentPlatforms,
            [FromQuery(Name = "ordering")] string? ordering,
            [FromQuery(Name = "search")] string? search)
        {
            this.logger.LogInformation("GetGamesAsync called to fetch games.");

            try
            {
                var result = await gamesService.GetGamesAsync(genres, parentPlatforms, ordering, search);
                this.logger.LogInformation("GetGamesAsync successfully fetched {Count} games.", result.count);
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

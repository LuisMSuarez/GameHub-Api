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

        public GamesController(ILogger<GamesController> logger)
        {
            this.logger = logger;
            this.gamesService = new GamesService();
        }

        [HttpGet(Name = "GetGames")]
        public async Task<CollectionResult<Game>> GetGamesAsync()
        {
            this.logger.LogInformation("GetGamesAsync called to fetch games.");

            try
            {
                var result = await gamesService.GetGamesAsync();
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

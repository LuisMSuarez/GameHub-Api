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

        public GamesController()
        {
            gamesService = new GamesService();
        }

        [HttpGet(Name = "GetGames")]
        public async Task<CollectionResult<Game>> GetGamesAsync()
        {
            return await gamesService.GetGamesAsync ();
        }
    }
}

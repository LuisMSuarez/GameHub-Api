using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameHubApi.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        [HttpGet(Name = "GetGames")]
        public CollectionResult<Game> Get()
        {
            return new CollectionResult<Game>
            {
                count = 20,
                next = "https://localhost",
                previous = null,
                results = Enumerable.Range(1, 20).Select(index => new Game
                {
                    id = index,
                    name = "Game " + index,
                    background_image = "https://media.rawg.io/media/games/618/618c2031a07bbff6b4f611f10b6bcdbc.jpg",
                    rating = Random.Shared.Next(1, 6),
                    metacritic = Random.Shared.Next(0, 101),
                    rating_top = Random.Shared.Next(1, 6)
                }).ToList<Game>()
            };
        }
    }
}

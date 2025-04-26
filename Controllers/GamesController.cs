using GameHubApi.Contracts;
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
                    Id = index,
                    Name = "Game " + index,
                    BackgroundIage = "https://media.rawg.io/media/games/618/618c2031a07bbff6b4f611f10b6bcdbc.jpg",
                    Rating = Random.Shared.Next(1, 6),
                    Metacritic = Random.Shared.Next(0, 101),
                    RatingTop = Random.Shared.Next(1, 6),
                    ParentPlatforms = new List<ParentPlatform>
                    {
                        new ParentPlatform
                        {
                            Platform = new Platform
                            {
                                Id = 1,
                                Name = "PC",
                                Slug = "pc"
                            }
                        }
                    }
                }).ToList<Game>()
            };
        }
    }
}

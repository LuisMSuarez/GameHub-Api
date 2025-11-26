namespace GameHubApi.Controllers
{
    using GameHubApi.Contracts;
    using GameHubApi.Services;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class UserGameController : ControllerBase
    {
        private readonly IUserGameService userGameService;
        private readonly ILogger<UserGameController> logger;
        public UserGameController(IUserGameService userGameService, ILogger<UserGameController> logger)
        {
            this.userGameService = userGameService ?? throw new ArgumentNullException(nameof(userGameService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        ////[HttpPost(Name = "userGames")]
        ////public Task<CollectionResult<Genre>> GetGenresAsync(
        ////  [FromQuery(Name = "page")] int page = 1,
        ////  [FromQuery(Name = "page_size")] int pageSize = 20)
        ////{
        ////    this.logger.LogInformation("GetGenresAsync called to fetch genres.");
        ////    return Ok();
        ////}
    }
}

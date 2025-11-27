namespace GameHubApi.Controllers
{
    using GameHubApi.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("v1/[controller]")]
    [Authorize(Policy = "CookiesAndBearer")]
    [ApiController]
    public class UserGamesController : ControllerBase
    {
        private readonly IUserGameService userGameService;
        private readonly ILogger<UserGamesController> logger;
        public UserGamesController(IUserGameService userGameService, ILogger<UserGamesController> logger)
        {
            this.userGameService = userGameService ?? throw new ArgumentNullException(nameof(userGameService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserGame([FromRoute] string id)
        {
            this.logger.LogInformation("GetUserGame called to fetch user game.");
            var userId = User.GetObjectId();
            var result = await this.userGameService.GetUserGame(id, userId);
            if (result == null)
            {
                this.logger.LogWarning("UserGame with id {Id} not found for user {UserId}.", id, userId);
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserGames()
        {
            this.logger.LogInformation("GetUserGames called to fetch user games.");
            var userId = User.GetObjectId();
            var result = await this.userGameService.GetUserGames(userId);
            return Ok(result);
        }
    }
}
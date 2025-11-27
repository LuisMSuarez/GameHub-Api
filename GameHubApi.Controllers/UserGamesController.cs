namespace GameHubApi.Controllers
{
    using GameHubApi.Contracts;
    using GameHubApi.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("v1/[controller]")]
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
        [Authorize(Policy = "CookiesAndBearer")]
        public async Task<IActionResult> GetUserGame([FromRoute] string id)
        {
            this.logger.LogInformation("GetUserGame called to fetch user game.");
            var user = HttpContext.User;
            var userId = user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            ArgumentException.ThrowIfNullOrEmpty(userId, "User ID claim is missing.");

            var result = await this.userGameService.GetUserGame(id, userId);
            if (result == null)
            {
                this.logger.LogWarning("UserGame with id {Id} not found for user {UserId}.", id, userId);
                return NotFound();
            }
            return Ok(result);
        }
    }
}
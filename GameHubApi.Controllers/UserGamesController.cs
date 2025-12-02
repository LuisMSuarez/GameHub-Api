namespace GameHubApi.Controllers
{
    using GameHubApi.Contracts;
    using GameHubApi.Services;
    using GameHubApi.Services.Exceptions;
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

            try
            {
                var result = await this.userGameService.GetUserGame(id, userId);
                if (result == null)
                {
                    this.logger.LogWarning("UserGame with id {Id} not found for user {UserId}.", id, userId);
                    return NotFound();
                }
                return Ok(result);
            }
            catch (ServiceException ex) when (ex.ResultCode == ServiceResultCode.NotFound)
            {
                this.logger.LogWarning(ex, "The requested resource was not found in GetUserGame.");
                return NotFound();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while fetching UserGame in GetUserGame.");
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserGames()
        {
            this.logger.LogInformation("GetUserGames called to fetch user games.");
            var userId = User.GetObjectId();

            try
            {
                var result = await this.userGameService.GetUserGames(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while fetching UserGames in GetUserGames.");
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserGame([FromRoute] string id, [FromBody] UserGame userGame)
        {
            this.logger.LogInformation("UpdateUserGame called for id {Id}.", id);
            var userId = User.GetObjectId();

            try
            {
                var updated = await this.userGameService.UpdateUserGame(id, userId, userGame);
                if (updated == null)
                {
                    this.logger.LogWarning("Failed to update UserGame with id {Id} for user {UserId}.", id, userId);
                    return NotFound();
                }

                return Ok(updated);
            }
            catch (ServiceException ex) when (ex.ResultCode == ServiceResultCode.NotFound)
            {
                this.logger.LogWarning(ex, "The requested resource was not found in UpdateUserGame.");
                return NotFound();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while updating UserGame in UpdateUserGame.");
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserGame([FromBody] UserGame userGame)
        {
            this.logger.LogInformation("CreateUserGame called.");
            var userId = User.GetObjectId();

            try
            {
                var created = await this.userGameService.CreateUserGame(userId, userGame);
                if (created == null)
                {
                    this.logger.LogWarning("Failed to create UserGame with GameId {Id} for user {UserId}.", userGame.GameId, userId);
                    return StatusCode(500, "Failed to create UserGame.");
                }

                return Created(created.Id!, created);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while creating UserGame in CreateUserGame.");
                throw;
            }
        }
    }
}
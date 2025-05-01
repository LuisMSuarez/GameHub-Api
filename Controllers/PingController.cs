namespace GameHubApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Route("v1/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly ILogger<PingController> logger;

        public PingController(ILogger<PingController> logger)
        {
            this.logger = logger;
        }

        [HttpGet(Name = "ping")]
        public IActionResult Ping()
        {
            this.logger.LogInformation("Ping called.");
            return Ok(new { message = "Request was successful!" });
        }
    }
}

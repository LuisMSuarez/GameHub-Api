using GameHubApi.Contracts;
using GameHubApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameHubApi.Controllers
{
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

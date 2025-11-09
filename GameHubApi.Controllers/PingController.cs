namespace GameHubApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// A lightweight health check endpoint for verifying that the API is responsive.
    /// Commonly used by monitoring tools, load balancers, or integration tests.
    /// </summary>
    [Route("v1/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly ILogger<PingController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PingController"/> class.
        /// </summary>
        /// <param name="logger">Logger used for diagnostics and observability.</param>
        public PingController(ILogger<PingController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Responds to GET requests with a simple success message.
        /// Useful for uptime checks and verifying API availability.
        /// </summary>
        /// <returns>HTTP 200 OK with a confirmation message.</returns>
        [HttpGet(Name = "ping")]
        public IActionResult Ping()
        {
            this.logger.LogInformation("Ping called.");
            return Ok(new { message = "Request was successful!" });
        }
    }
}

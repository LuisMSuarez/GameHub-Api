using GameHubApi.Contracts;
using GameHubApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameHubApi.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenresService genresService;
        private readonly ILogger<GenresController> logger;

        public GenresController(ILogger<GenresController> logger, IGenresService genresService)
        {
            this.genresService = genresService ?? throw new ArgumentNullException(nameof(genresService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet(Name = "genres")]
        public Task<CollectionResult<Genre>> GetGenresAsync(
            [FromQuery(Name = "page")] int page = 1,
            [FromQuery(Name = "page_size")] int pageSize = 20)
        {
            this.logger.LogInformation("GetGenresAsync called to fetch genres.");

            try
            {
                return this.genresService.GetGenresAsync(page, pageSize);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while fetching genres in GetGenresAsync.");
                throw; // Re-throw the exception to ensure proper error handling
            }
        }
    }
}

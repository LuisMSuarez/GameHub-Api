using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameHubApi.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize(AuthenticationSchemes = "Microsoft")]
    public class AdminController : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult GetTestData()
        {
            var user = HttpContext.User;
            var name = user.FindFirst(ClaimTypes.Name)?.Value ?? user.FindFirst("name")?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value
                ?? user.FindFirst("preferred_username")?.Value; // fallback for some Microsoft accounts
            var objectId = user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            return Ok(new
            {
                name,
                email,
                objectId
            });
        }
    }
}
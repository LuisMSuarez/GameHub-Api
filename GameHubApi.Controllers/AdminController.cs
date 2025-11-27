using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameHubApi.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class AdminController : ControllerBase
    {
        [HttpGet("login")]
        [Authorize]

        // This endpoint is intended for browsers only.
        // It will trigger DefaultChallengeScheme, the authentication process using OpenIdConnect flow if the user is not authenticated.
        // It will then set a cookie upon successful authentication.
        // From this point on, the endpoints that use Cookies authentication scheme will work.
        // The endpoints that allow both Cookies and Bearer authentication schemes only work if the user has a valid cookie or a valid Bearer token.
        // They will not trigger the authentication process.
        public IActionResult Login()
        {
            var name = User.FindFirst("name")?.Value;
            var email = User.FindFirst("preferred_username")?.Value;
            var objectId = User.GetObjectId();
            return Ok(new
            {
                name,
                email,
                objectId
            });
        }

        [HttpGet("test")]
        [Authorize(Policy = "CookiesAndBearer")]
        public IActionResult GetTestData()
        {
            return Ok(new
            { 
                message = "You have accessed a protected endpoint using either Cookies or Bearer authentication." 
            });
        }
    }
}
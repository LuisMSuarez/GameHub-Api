using System.Security.Claims;

namespace GameHubApi.Controllers
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetObjectId(this ClaimsPrincipal user)
            => user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
               ?? throw new ArgumentException("User ID claim is missing.");
    }
}

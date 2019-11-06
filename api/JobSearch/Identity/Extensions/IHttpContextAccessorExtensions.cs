namespace JobSearch.Identity.Extensions
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;

    public static class IHttpContextAccessorExtensions
    {
        public static string CurrentUserId(this IHttpContextAccessor httpContextAccessor)
        {
            var stringId = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return stringId ?? "0";
        }
    }
}
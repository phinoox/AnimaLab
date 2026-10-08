using System.Security.Claims;
using Anima.Core.Models.Authentication;
using Anima.Data.Contexts;

namespace Anima.Api.MiddleWare;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthMiddleware> _logger;

    public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IUserContext userContext, AppDbContext dbContext)
    {
        var claimsPrincipal = context.User;

        if (claimsPrincipal != null && claimsPrincipal.Identity?.IsAuthenticated == true)
        {
            // 1. Extract User ID from JWT NameIdentifier claim
            var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
            {
                // 2. Hydrate the Scoped UserContext
                userContext.UserId = userId;

                // 3. Lazy-load/Hydrate the actual User entity from DB
                // This prevents N+1 queries in your controllers later!
                var user = await dbContext.Users.FindAsync(userId);
                userContext.CurrentUser = user;

                // 4. Extract Roles from JWT claims
                userContext.Roles = claimsPrincipal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

                _logger.LogDebug("UserContext hydrated for User: {UserId}", userId);
            }
        }

        await _next(context);
    }
}
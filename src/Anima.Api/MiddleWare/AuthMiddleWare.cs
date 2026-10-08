using System.Security.Claims;
using Anima.Core.Models.Authentication;
using Anima.Data.Contexts;

namespace Anima.Api.MiddleWare;

/// <summary>
/// Middleware responsible for intercepting incoming HTTP requests to hydrate the <see cref="IUserContext"/>.
/// It extracts user identity and roles from the authentication claims and pops the user entity into the scoped context.
/// </summary>
public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next delegate in the request pipeline.</param>
    /// <param name="logger">The logger for recording middleware-level events and errors.</param>
    public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware logic to populate the user context from the current HTTP request's claims.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="userContext">The scoped user context to be hydrated.</param>
    /// <param name="dbContext">The application database context for hydrating the user entity.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
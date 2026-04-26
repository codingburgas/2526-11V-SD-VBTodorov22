using MovieSeriesCatalog.Data;

namespace MovieSeriesCatalog.Middleware;

public class AdminFilmApiAuthMiddleware
{
    private readonly RequestDelegate _next;

    public AdminFilmApiAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!RequiresAdminAuthorization(context.Request))
        {
            await _next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated != true)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = "Authentication is required to upload a cover image." });
            return;
        }

        if (!context.User.IsInRole(RoleNames.Admin))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { message = "Only admins can upload cover images." });
            return;
        }

        await _next(context);
    }

    private static bool RequiresAdminAuthorization(HttpRequest request)
    {
        if (!HttpMethods.IsPost(request.Method))
        {
            return false;
        }

        return request.Path.StartsWithSegments("/admin/films", out var remainingPath)
            && remainingPath.Value?.EndsWith("/cover", StringComparison.OrdinalIgnoreCase) == true;
    }
}

namespace BoilerplateApp.Web.Services;

public sealed class UserActivityTrackingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IUserActivityTracker tracker)
    {
        await next(context);

        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        if (!HttpMethods.IsGet(context.Request.Method))
        {
            return;
        }

        var path = context.Request.Path;
        // Skip framework/internal Blazor requests so only actual page navigation updates last-seen.
        if (path.StartsWithSegments("/_framework") || path.StartsWithSegments("/_blazor"))
        {
            return;
        }

        await tracker.TrackAsync(context.User, context.RequestAborted);
    }
}
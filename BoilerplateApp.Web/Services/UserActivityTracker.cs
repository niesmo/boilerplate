using System.Security.Claims;
using BoilerplateApp.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace BoilerplateApp.Web.Services;

public interface IUserActivityTracker
{
    Task TrackAsync(ClaimsPrincipal principal, CancellationToken cancellationToken);
}

public sealed class UserActivityTracker(
    UserManager<ApplicationUser> userManager,
    IMemoryCache cache,
    ILogger<UserActivityTracker> logger) : IUserActivityTracker
{
    private static readonly TimeSpan UpdateInterval = TimeSpan.FromMinutes(5);

    public async Task TrackAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        var userId = userManager.GetUserId(principal);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return;
        }

        var cacheKey = $"last-seen:{userId}";
        // Cache acts as a per-user throttle so we skip repeated updates and avoid writing
        // last-seen on every request while still keeping recent activity reasonably fresh.
        if (cache.TryGetValue(cacheKey, out _))
        {
            return;
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return;
        }

        user.LastSeenAtUtc = DateTime.UtcNow;
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            logger.LogWarning("Failed to update last-seen for user {UserId}", userId);
            return;
        }

        cache.Set(cacheKey, true, UpdateInterval);
    }
}
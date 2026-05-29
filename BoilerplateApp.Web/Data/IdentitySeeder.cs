using Microsoft.AspNetCore.Identity;

namespace BoilerplateApp.Web.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await EnsureRoleAsync(roleManager, RoleConstants.Admin);
        await EnsureRoleAsync(roleManager, RoleConstants.User);

        var email = configuration["AdminBootstrap:Email"];
        var password = configuration["AdminBootstrap:Password"];
        var displayName = configuration["AdminBootstrap:DisplayName"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning(
                "Admin bootstrap was skipped because AdminBootstrap:Email or AdminBootstrap:Password is not configured.");
            return;
        }

        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                DisplayName = displayName,
                SignedUpAtUtc = DateTime.UtcNow,
                LastSeenAtUtc = DateTime.UtcNow,
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(error => error.Description));
                logger.LogError("Failed to create admin bootstrap user: {Errors}", errors);
                return;
            }
        }

        await EnsureRoleAssignmentAsync(userManager, user, RoleConstants.User);
        await EnsureRoleAssignmentAsync(userManager, user, RoleConstants.Admin);
    }

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string role)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task EnsureRoleAssignmentAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        string role)
    {
        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
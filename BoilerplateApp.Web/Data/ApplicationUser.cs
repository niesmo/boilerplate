using Microsoft.AspNetCore.Identity;

namespace BoilerplateApp.Web.Data;

public sealed class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public DateTime SignedUpAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastSeenAtUtc { get; set; }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BoilerplateApp.Web.Data;

/// <summary>
/// Allows EF Core CLI tools (dotnet ef migrations) to create the DbContext
/// without requiring the Aspire AppHost to be running.
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=contactdb;Username=postgres;Password=postgres");
        return new AppDbContext(optionsBuilder.Options);
    }
}

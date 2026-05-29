using Aspire.Hosting;
using Aspire.Hosting.Testing;

namespace BoilerplateApp.Web.Tests;

public sealed class AspireAppHostFixture : IAsyncLifetime
{
    private DistributedApplication? app;

    public DistributedApplication App => app ?? throw new InvalidOperationException("The distributed application has not been initialized.");

    public async Task InitializeAsync()
    {
        Environment.CurrentDirectory = Projects.BoilerplateApp_AppHost.ProjectPath;

        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.BoilerplateApp_AppHost>();
        app = await builder.BuildAsync();

        await app.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (app is null)
        {
            return;
        }

        await app.StopAsync();

        if (app is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }

        app = null;
    }
}

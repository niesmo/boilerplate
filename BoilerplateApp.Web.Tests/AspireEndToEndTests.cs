using Aspire.Hosting.Testing;

namespace BoilerplateApp.Web.Tests;

[Collection(AspireE2ETestCollection.Name)]
public sealed class AspireEndToEndTests(AspireAppHostFixture fixture)
{
    [Fact]
    public async Task Home_page_returns_success()
    {
        using var client = fixture.App.CreateHttpClient("webfrontend");
        using var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Hello, world!", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Sign_in_page_returns_success()
    {
        using var client = fixture.App.CreateHttpClient("webfrontend");
        using var response = await client.GetAsync("/signin");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Sign in", content, StringComparison.Ordinal);
    }
}

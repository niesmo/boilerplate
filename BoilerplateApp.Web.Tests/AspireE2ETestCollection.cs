namespace BoilerplateApp.Web.Tests;

[CollectionDefinition(Name)]
public sealed class AspireE2ETestCollection : ICollectionFixture<AspireAppHostFixture>
{
    public const string Name = "Aspire E2E collection";
}

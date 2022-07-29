namespace MiauTests.Fixtures;

/// <summary>
/// This class has no code, and is never created. Its purpose is simply to be the place
/// to apply [CollectionDefinition] and all the <see cref="ICollectionFixture{TFixture}"/>
/// interfaces.
/// </summary>
/// <remarks>See: https://stackoverflow.com/questions/12976319/xunit-net-global-setup-teardown</remarks>
[CollectionDefinition(nameof(ServicesFixture))]
public sealed class ServicesFixtureCollection : ICollectionFixture<ServicesFixture>
{
}
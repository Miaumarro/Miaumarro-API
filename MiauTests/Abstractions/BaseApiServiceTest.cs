namespace MiauTests.Abstractions;

/// <summary>
/// Provides a base for test classes that need to interact with
/// the API's Miau services in the <see cref="IServiceProvider"/>.
/// </summary>
[Collection(nameof(ServicesFixture))]
public abstract class BaseApiServiceTest : IDisposable
{
    /// <summary>
    /// The service scope.
    /// </summary>
    /// <remarks>Use this to resolve all services you need.</remarks>
    protected IServiceScope Scope { get; }

    // Test classes are created every single time a test is run,
    // so we create a clean test context in the constructor and
    // dispose in the Dispose() method.
    // See: https://xunit.net/docs/shared-context
    public BaseApiServiceTest(ServicesFixture fixture)
        => Scope = fixture.ServiceProvider.CreateScope();

    /// <summary>
    /// Disposes the API's <see cref="IServiceScope"/> used to
    /// resolve the services in this test class.
    /// </summary>
    public virtual void Dispose()
    {
        Scope.Dispose();
        GC.SuppressFinalize(this);
    }
}
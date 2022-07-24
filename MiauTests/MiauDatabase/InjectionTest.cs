using MiauDatabase;
using MiauDatabase.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MiauTests.MiauDatabase;

public sealed class InjectionTest
{
    private readonly IServiceProvider _ioc;
    private readonly IServiceScopeFactory _scopeFactory;

    public InjectionTest()
    {
        _ioc = new ServiceCollection()
            .AddMiauDb("Data Source=:memory:;New=True;", false)
            .BuildServiceProvider();

        _scopeFactory = _ioc.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    internal MiauDbContext MiauDbInjectionTest()
    {
        using var scope = _scopeFactory.CreateScope();

        // This must not throw
        return scope.ServiceProvider.GetRequiredService<MiauDbContext>();
    }
}

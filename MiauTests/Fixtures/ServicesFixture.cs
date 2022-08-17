using LinqToDB.EntityFrameworkCore;
using MiauAPI.Extensions;
using MiauAPI.Models.Requests;
using MiauAPI.Validators;
using MiauAPI.Validators.Abstractions;
using MiauDatabase.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace MiauTests.Fixtures;

/// <summary>
/// Fixture for the API's <see cref="IServiceProvider"/>.
/// </summary>
/// <remarks>
/// This class creates one service provider before any of the tests are run and
/// disposes of it when the tests are over. The same service provider is shared
/// among all test classes that inherit from <see cref="BaseApiServiceTest"/>
/// or that have the <see cref="CollectionAttribute"/> with this class' type
/// name applied to it.
/// </remarks>
public sealed class ServicesFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    /// <summary>
    /// A replica of the API's service provider.
    /// </summary>
    internal IServiceProvider ServiceProvider { get; }

    public ServicesFixture()
    {
        // Enable LinqToDb extensions
        LinqToDBForEFTools.Initialize();

        _serviceProvider = WebApplication.CreateBuilder()               // Add the default services some Miau services may depend on
            .Services
            .AddLogging(x => x.ClearProviders())                        // Supress service logging, if any is being used
            .AddMiauServices()                                          // Add Miau services
            .AddMiauDb("Data Source=file::memory:?cache=shared;", true) // Initialize an in-memory SQLite database
            .AddSingleton<IRequestValidator<UserAuthenticationRequest>, UserAuthenticationRequestValidator>()
            .BuildServiceProvider();

        ServiceProvider = _serviceProvider;
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
        GC.SuppressFinalize(this);
    }
}
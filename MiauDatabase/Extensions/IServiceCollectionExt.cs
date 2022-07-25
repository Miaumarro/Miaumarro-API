using MiauDatabase.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MiauDatabase.Extensions;

/// <summary>
/// Contains extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class IServiceCollectionExt
{
    /// <summary>
    /// Adds <see cref="MiauDbContext"/> as a scoped service to this IoC.
    /// </summary>
    /// <param name="serviceCollection">This service collection.</param>
    /// <param name="migrate"><see langword="true"/> if a migration should be performed, <see langword="false"/> otherwise.</param>
    /// <returns>This service collection.</returns>
    public static IServiceCollection AddMiauDb(this IServiceCollection serviceCollection, bool migrate = true)
    {
        // Perform the migration
        // If the database doesn't exist, create it
        if (migrate)
        {
            using var dbContext = new MiauDbContext(MiauDbStatics.MiauDbDefaultOptions);
            dbContext.Database.Migrate();
        }

        // Add the database context to the IoC
        serviceCollection.AddDbContext<MiauDbContext>(options => MiauDbStatics.GetDefaultDbOptions(options));

        return serviceCollection;
    }
}
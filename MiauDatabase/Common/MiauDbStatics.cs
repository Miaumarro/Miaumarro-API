using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MiauDatabase.Common;

/// <summary>
/// Contains common <see langword="static"/> objects related to <see cref="MiauDbContext"/>'s initialization.
/// </summary>
internal static class MiauDbStatics
{
    /// <summary>
    /// SQLite database connection string.
    /// </summary>
    /// <remarks>Points to the current directory of the application. Has the format "Data Source=Miau.db"</remarks>
    internal static readonly string MiauDbConnectionString = "Data Source=" + Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location)?.FullName ?? string.Empty, "Miau.db");

    /// <summary>
    /// Default options for a <see cref="MiauDbContext"/>
    /// </summary>
    internal static DbContextOptions<MiauDbContext> MiauDbDefaultOptions { get; } = new DbContextOptionsBuilder<MiauDbContext>()
        .UseSnakeCaseNamingConvention()     // Set column names to snake_case format
        .UseSqlite(MiauDbConnectionString)  // Database connection string
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) // Disable EF Core entity tracking - see https://docs.microsoft.com/en-us/ef/core/change-tracking/
        .Options;

    /// <summary>
    /// Gets a database options builder, adds the default settings, and returns it.
    /// </summary>
    /// <param name="options">The database options to have the default settings applied to.</param>
    /// <returns>A database options with the default settings applied.</returns>
    internal static DbContextOptionsBuilder GetDefaultDbOptions(DbContextOptionsBuilder options)
    {
        return options.UseSnakeCaseNamingConvention()   // Set column names to snake_case format
            .UseSqlite(MiauDbConnectionString)          // Database connection string
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);    // Disable EF Core entity tracking - see https://docs.microsoft.com/en-us/ef/core/change-tracking/
    }
}
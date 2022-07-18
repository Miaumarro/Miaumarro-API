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
    private static readonly string _miauDbConnectionString = "Data Source=" + Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location)?.FullName ?? string.Empty, "Miau.db");

    /// <summary>
    /// Default options builder for a <see cref="MiauDbContext"/>
    /// </summary>
    internal static DbContextOptionsBuilder<MiauDbContext> MiauDbOptionsBuilder { get; } = new DbContextOptionsBuilder<MiauDbContext>()
        .UseSnakeCaseNamingConvention()     // Set column names to snake_case format
        .UseSqlite(_miauDbConnectionString) // Database connection string
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);    // Disable EF Core entity tracking - see https://docs.microsoft.com/en-us/ef/core/change-tracking/
}
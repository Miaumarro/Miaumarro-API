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
    private static readonly string _miauDbConnectionString = "Data Source=" + Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location)?.FullName ?? string.Empty, "Miau.db");

    /// <summary>
    /// Default options builder for a <see cref="MiauDbContext"/>
    /// </summary>
    internal static DbContextOptionsBuilder<MiauDbContext> MiauDbOptionsBuilder { get; } = new DbContextOptionsBuilder<MiauDbContext>()
        .UseSnakeCaseNamingConvention()
        .UseSqlite(_miauDbConnectionString)  // Database connection string
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
}
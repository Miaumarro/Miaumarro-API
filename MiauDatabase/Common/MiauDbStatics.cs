using Microsoft.EntityFrameworkCore;

namespace MiauDatabase.Common;

/// <summary>
/// Contains common <see langword="static"/> objects related to <see cref="MiauDbContext"/>'s initialization.
/// </summary>
internal static class MiauDbStatics
{
    /// <summary>
    /// Default options builder for a <see cref="MiauDbContext"/>
    /// </summary>
    internal static DbContextOptionsBuilder<MiauDbContext> MiauDbOptionsBuilder { get; } = new DbContextOptionsBuilder<MiauDbContext>()
        .UseSnakeCaseNamingConvention()
        .UseSqlite("Data Source=Miau.db;")  // Database connection string
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
}
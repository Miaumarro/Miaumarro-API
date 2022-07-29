using MiauDatabase.Common;
using Microsoft.EntityFrameworkCore.Design;

namespace MiauDatabase.Config;

/// <summary>
/// This class is only used at design time, when EF Core is asked to perform a migration.
/// </summary>
internal sealed class MiauDbContextFactory : IDesignTimeDbContextFactory<MiauDbContext>
{
    public MiauDbContext CreateDbContext(string[] args)
        => new(MiauDbStatics.GetDefaultDbOptions<MiauDbContext>().Options);
}
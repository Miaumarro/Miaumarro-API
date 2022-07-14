using MiauDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiauDatabase.Config;

/// <summary>
/// Configures relationships for <see cref="PurchaseEntity"/>.
/// </summary>
internal sealed class PurchaseEntityTypeConfiguration : IEntityTypeConfiguration<PurchaseEntity>
{
    public void Configure(EntityTypeBuilder<PurchaseEntity> builder)
    {
        // Purchases -> Purchased Products
        builder.HasMany(x => x.PurchasedProductRel)
            .WithOne(x => x.PurchaseRel)
            .HasForeignKey(x => x.PurchaseIdFK)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
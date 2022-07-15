using MiauDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiauDatabase.Config;

/// <summary>
/// Configures relationships for <see cref="CouponEntity"/>.
/// </summary>
internal sealed class CouponEntityTypeConfiguration : IEntityTypeConfiguration<CouponEntity>
{
    public void Configure(EntityTypeBuilder<CouponEntity> builder)
    {
        // Coupons -> Purchases
        builder.HasMany(x => x.Purchases)
            .WithOne(x => x.Coupon)
            .HasForeignKey(x => x.Coupon!.Id)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
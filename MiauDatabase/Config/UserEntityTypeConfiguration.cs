using MiauDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiauDatabase.Config;

/// <summary>
/// Configures relationships for <see cref="UserEntity"/>.
/// </summary>
internal sealed class UserEntityTypeConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        // Users -> Addresses
        builder.HasMany(x => x.AddressesRel)
            .WithOne(x => x.UserRel)
            .HasForeignKey(x => x.UserIdFK)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        // Users -> Pets
        builder.HasMany(x => x.PetsRel)
            .WithOne(x => x.UserRel)
            .HasForeignKey(x => x.UserIdFK)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        // Users -> Purchases
        builder.HasMany(x => x.PurchasesRel)
            .WithOne(x => x.UserRel)
            .HasForeignKey(x => x.UserIdFK)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        // Users -> Reviews
        builder.HasMany(x => x.ProductReviewsRel)
            .WithOne(x => x.UserRel)
            .HasForeignKey(x => x.UserIdFK)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.SetNull);

        // Users -> Wishlist
        builder.HasMany(x => x.WishlistRel)
            .WithOne(x => x.UserRel)
            .HasForeignKey(x => x.UserIdFK)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
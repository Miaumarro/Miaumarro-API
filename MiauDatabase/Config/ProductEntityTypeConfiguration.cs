using MiauDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiauDatabase.Config;

/// <summary>
/// Configures relationships for <see cref="ProductEntity"/>.
/// </summary>
internal sealed class ProductEntityTypeConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        // Products -> Product Images
        builder.HasMany(x => x.ProductImages)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.Product.Id)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);

        // Products -> Product Reviews
        builder.HasMany(x => x.ProductReviews)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.Product.Id)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        // Products -> Purchased Products
        builder.HasMany(x => x.PurchasedProducts)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.Product.Id)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict); // Should we restrict deletion on purchased products?

        // Products -> Product Reviews
        builder.HasMany(x => x.Wishlist)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.Product.Id)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
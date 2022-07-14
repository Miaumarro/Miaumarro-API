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
        builder.HasMany(x => x.ProductImagesRel)
            .WithOne(x => x.ProductRel)
            .HasForeignKey(x => x.ProductIdFK)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);

        // Products -> Product Reviews
        builder.HasMany(x => x.ProductReviewsRel)
            .WithOne(x => x.ProductRel)
            .HasForeignKey(x => x.ProductIdFK)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        // Products -> Purchased Products
        builder.HasMany(x => x.PurchasedProductsRel)
            .WithOne(x => x.ProductRel)
            .HasForeignKey(x => x.ProductIdFK)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict); // Should we restrict deletion on purchased products?

        // Products -> Product Reviews
        builder.HasMany(x => x.WishlistRel)
            .WithOne(x => x.ProductRel)
            .HasForeignKey(x => x.ProductIdFK)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
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
        // Cpf must be unique
        builder.HasIndex(x => x.Cpf)
            .IsUnique();

        // E-mail must be unique
        builder.HasIndex(x => x.Email)
            .IsUnique();

        // Users -> Addresses
        builder.HasMany(x => x.Addresses)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.User.Id)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        // Users -> Pets
        builder.HasMany(x => x.Pets)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.User.Id)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        // Users -> Purchases
        builder.HasMany(x => x.Purchases)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.User.Id)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        // Users -> Reviews
        builder.HasMany(x => x.ProductReviews)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.User!.Id)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.SetNull);

        // Users -> Wishlist
        builder.HasMany(x => x.Wishlist)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.User.Id)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
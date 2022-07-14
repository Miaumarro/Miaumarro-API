using MiauDatabase.Common;
using MiauDatabase.Entities;
using Microsoft.EntityFrameworkCore;

namespace MiauDatabase;

/// <summary>
/// Represents a database for the Miaumarro Petshop.
/// </summary>
public sealed class MiauDbContext : DbContext
{
    public DbSet<AddressEntity> Addresses { get; init; } = null!;
    public DbSet<AppointmentEntity> Appointments { get; init; } = null!;
    public DbSet<PetEntity> Pets { get; init; } = null!;
    public DbSet<ProductEntity> Products { get; init; } = null!;
    public DbSet<ProductImageEntity> ProductImages { get; init; } = null!;
    public DbSet<ProductReviewEntity> ProductReviews { get; init; } = null!;
    public DbSet<PurchasedProductEntity> PurchasedProducts { get; init; } = null!;
    public DbSet<PurchaseEntity> Purchases { get; init; } = null!;
    public DbSet<UserEntity> Users { get; init; } = null!;
    public DbSet<WishlistEntity> Wishlist { get; init; } = null!;

    public MiauDbContext() : this(MiauDbStatics.MiauDbOptionsBuilder.Options)
    {
    }

    public MiauDbContext(DbContextOptions<MiauDbContext> options) : base(options)
    {
    }
}
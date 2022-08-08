using MiauDatabase.Abstractions;
using MiauDatabase.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MiauDatabase.Entities;

/// <summary>
/// Represents a database store product.
/// </summary>
[Comment("Represents a store product.")]
public sealed record ProductEntity : MiauDbEntity
{
    private decimal _discount = 0;
    private string? _brand;

    /// <summary>
    /// The product images associated with this product.
    /// </summary>
    public List<ProductImageEntity> ProductImages { get; init; } = new();

    /// <summary>
    /// The product reviews associated with this product.
    /// </summary>
    public List<ProductReviewEntity> ProductReviews { get; init; } = new();

    /// <summary>
    /// The purchased products associated with this product.
    /// </summary>
    public List<PurchasedProductEntity> PurchasedProducts { get; init; } = new();

    /// <summary>
    /// The wishlist items associated with this product.
    /// </summary>
    public List<WishlistEntity> Wishlist { get; init; } = new();

    /// <summary>
    /// This product's name.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// This product's description.
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = null!;

    /// <summary>
    /// This product's price.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Determines whether this product is available for sale.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// This product's amount in stock.
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// This product's store tags.
    /// </summary>
    public ProductTag Tags { get; set; }

    /// <summary>
    /// This product's brand.
    /// </summary>
    [MaxLength(30)]
    public string? Brand
    {
        get => _brand;
        set => _brand = value?.ToLowerInvariant();
    }

    /// <summary>
    /// This product's discount.
    /// </summary>
    /// <remarks>Must be a value between 0 and 1.</remarks>
    public decimal Discount
    {
        get => _discount;
        set => _discount = (value is < 0 or > 1) ? throw new ArgumentOutOfRangeException(nameof(value), "Discount must be between 0 and 1") : value;
    }
}
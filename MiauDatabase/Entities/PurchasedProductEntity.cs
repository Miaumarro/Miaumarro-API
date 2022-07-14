using MiauDatabase.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MiauDatabase.Entities;

/// <summary>
/// Represents a database purchased product.
/// </summary>
/// <value></value>
[Comment("Represents a purchased product.")]
public sealed record PurchasedProductEntity : MiauDbEntity
{
    /// <summary>
    /// The product this purchased product is associated with.
    /// </summary>
    public ProductEntity FkProduct { get; init; } = null!;

    /// <summary>
    /// The purchase this purchased product is associated with.
    /// </summary>
    public PurchaseEntity FkPurchase { get; init; } = null!;

    /// <summary>
    /// How much the purchased product was sold for.
    /// </summary>
    public decimal SalePrice { get; init; }
}
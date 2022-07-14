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
    public ProductEntity ProductRel { get; init; } = null!;

    /// <summary>
    /// The purchase this purchased product is associated with.
    /// </summary>
    public PurchaseEntity PurchaseRel { get; init; } = null!;

    /// <summary>
    /// The ID of the product associated with this purchased product.
    /// </summary>
    public int ProductIdFK { get; init; }

    /// <summary>
    /// The ID of the purchase associated with this purchased product.
    /// </summary>
    public int PurchaseIdFK { get; init; }

    /// <summary>
    /// How much the purchased product was sold for.
    /// </summary>
    public decimal SalePrice { get; init; }
}
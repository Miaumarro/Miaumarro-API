using MiauDatabase.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MiauDatabase.Entities;

/// <summary>
/// Represents a database product image.
/// </summary>
/// <value></value>
[Comment("Represents a product image.")]
public sealed record ProductImageEntity : MiauDbEntity
{
    /// <summary>
    /// The product this product image is associated with.
    /// </summary>
    public ProductEntity ProductRel { get; init; } = null!;

    /// <summary>
    /// The ID of the product associated with this product image.
    /// </summary>
    public int ProductIdFK { get; init; }

    /// <summary>
    /// The location of the image in the file system.
    /// </summary>
    public string FileUrl { get; init; } = null!;
}
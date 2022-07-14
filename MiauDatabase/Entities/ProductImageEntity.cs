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
    public ProductEntity Product { get; init; } = null!;

    /// <summary>
    /// The location of the image in the file system.
    /// </summary>
    public string FileUrl { get; init; } = null!;
}
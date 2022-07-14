using MiauDatabase.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MiauDatabase.Entities;

/// <summary>
/// Represents a database wishlist item.
/// </summary>
[Comment("Represents a wishlist item.")]
public sealed record WishlistEntity : MiauDbEntity
{
    /// <summary>
    /// The user this wishlist item is associated with.
    /// </summary>
    public UserEntity FkUser { get; init; } = null!;

    /// <summary>
    /// The product this wishlist item is associated with.
    /// </summary>
    public ProductEntity FkProduct { get; init; } = null!;
}
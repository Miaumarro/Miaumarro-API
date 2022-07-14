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
    public UserEntity UserRel { get; init; } = null!;

    /// <summary>
    /// The product this wishlist item is associated with.
    /// </summary>
    public ProductEntity ProductRel { get; init; } = null!;

    /// <summary>
    /// The ID of the user associated with this wishlist item.
    /// </summary>
    public int UserIdFK { get; init; }

    /// <summary>
    /// The ID of the product associated with this wishlist item.
    /// </summary>
    public int ProductIdFK { get; init; }
}
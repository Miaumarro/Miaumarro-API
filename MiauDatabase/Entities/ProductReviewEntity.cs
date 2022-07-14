using MiauDatabase.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MiauDatabase.Entities;

/// <summary>
/// Represents a database product image.
/// </summary>
/// <value></value>
[Comment("Represents a product image.")]
public sealed record ProductReviewEntity : MiauDbEntity
{
    private int _score;

    /// <summary>
    /// The user this product review is associated with.
    /// </summary>
    public UserEntity? UserRel { get; init; }

    /// <summary>
    /// The product this product review is associated with.
    /// </summary>
    public ProductEntity ProductRel { get; init; } = null!;

    /// <summary>
    /// The ID of the user associated with this product review.
    /// </summary>
    public int? UserIdFK { get; init; }

    /// <summary>
    /// The ID of the product associated with this product review.
    /// </summary>
    public int ProductIdFK { get; init; }

    /// <summary>
    /// The review of the product.
    /// </summary>
    public string Description { get; init; } = null!;

    /// <summary>
    /// The review score given to the reviewed product.
    /// </summary>
    public int Score
    {
        get => _score;
        init => _score = (value is < 0 or > 5) ? throw new ArgumentOutOfRangeException(nameof(value), "Score must be between 0 and 5") : value;
    }
}
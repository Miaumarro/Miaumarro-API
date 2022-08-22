using MiauDatabase.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MiauDatabase.Entities;

/// <summary>
/// Represents a database product review.
/// </summary>
/// <value></value>
[Comment("Represents a product review.")]
public sealed record ProductReviewEntity : MiauDbEntity
{
    private int _score;

    /// <summary>
    /// The user this product review is associated with.
    /// </summary>
    public UserEntity User { get; init; } = null!;

    /// <summary>
    /// The product this product review is associated with.
    /// </summary>
    public ProductEntity Product { get; init; } = null!;

    /// <summary>
    /// The review of the product.
    /// </summary>
   [MaxLength(500)]
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
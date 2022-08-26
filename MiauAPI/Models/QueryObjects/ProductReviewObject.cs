using MiauDatabase.Enums;

namespace MiauAPI.Models.QueryObjects;

/// <summary>
/// Represents the object in a product review query.
/// </summary>
/// <param name="Id">The id of the user review.</param>
/// <param name="UserId">The id of the user.</param>
/// <param name="ProductId">The id of the product.</param>
/// <param name="Description">Description of the review.</param>
/// <param name="Score">The product score.</param>
public sealed record ProductReviewObject
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public int ProductId { get; init; }
    public string? Description { get; init; }
    public int Score { get; init; }
}
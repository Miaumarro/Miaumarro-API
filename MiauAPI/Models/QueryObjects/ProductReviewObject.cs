using MiauDatabase.Enums;

namespace MiauAPI.Models.QueryObjects;

/// <summary>
/// Represents the object in a product review query.
/// </summary>
/// <param name="Id">The id of the user review.</param>
/// <param name="ProductId">The id of the product.</param>
/// <param name="UserId">The id of the user.</param>
/// <param name="Description">Description of the review.</param>
/// <param name="Score">The product score.</param>
public sealed record ProductReviewObject(int Id, int ProductId, int? UserId, string? Description, int Score);
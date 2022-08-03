namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for creation of a new product review.
/// </summary>
/// <param name="UserId">The user this product review is associated with.</param>
/// <param name="ProductId">The product this product review is associated with.</param>
/// <param name="Description">The review of the product.</param>
/// <param name="Score">The review score given to the reviewed product.</param>
public sealed record CreatedProductReviewRequest(int? UserId, int ProductId, string Description, int Score);
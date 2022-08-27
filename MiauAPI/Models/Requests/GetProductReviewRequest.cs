namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for getting a product review.
/// </summary>
/// <param name="ProductId">The Id of the reviewed product.</param>
/// <param name="UserId">The Id of the user who made the review.</param>
public sealed record GetProductReviewRequest(int ProductId, int? UserId);
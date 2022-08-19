using MiauAPI.Models.QueryObjects;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response when a review is requested by its Id.
/// </summary>
/// <param name="ProductReview">The resulted review.</param>
public sealed record GetProductReviewByIdResponse(ProductReviewObject ProductReview);

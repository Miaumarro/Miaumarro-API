using MiauAPI.Models.QueryObjects;
using MiauAPI.Pagination;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a review query is successfully executed.
/// </summary>
/// <param name="ProductReviews">The resulted list of review.</param>
public sealed record GetProductReviewResponse(PagedList<ProductReviewObject> ProductReviews);

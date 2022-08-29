namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Set relevant parameters for a review search.
/// </summary>
/// <param name="ProductId">The Id of the prodct being reviewed.</param>
public sealed record ProductReviewParameters(int ProductId) : QueryStringParameters;
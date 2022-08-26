namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Set relevant parameters for a review search.
/// </summary>
/// <param name="UserId">The id of the user related to the review.</param>
public sealed record ProductReviewParameters(int UserId) : QueryStringParameters;
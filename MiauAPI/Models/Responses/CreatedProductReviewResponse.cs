namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a review is successfully created.
/// </summary>
/// <param name="Id">The database ID of the review.</param>
public sealed record CreatedProductReviewResponse(int Id);

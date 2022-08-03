namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a product review is successfully created.
/// </summary>
/// <param name="Id">The database ID of the product review.</param>
public sealed record CreatedProductReviewResponse(int Id);
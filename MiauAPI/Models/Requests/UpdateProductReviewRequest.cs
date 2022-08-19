using MiauDatabase.Enums;

namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for update a review by a given Id.
/// </summary>
/// <param name="Id">The id of the user review.</param>
/// <param name="UserId">The id of the user.</param>
/// <param name="ProductId">The id of the product.</param>
/// <param name="Description">Description of the review.</param>
/// <param name="Score">The product score.</param>
public sealed record UpdateProductReviewRequest(int Id, int UserId, int ProductId, string Description, int Score);

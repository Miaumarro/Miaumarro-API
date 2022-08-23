using MiauDatabase.Enums;

namespace MiauAPI.Models.QueryObjects;

/// <summary>
/// Represents the object in a product review query.
/// </summary>
/// <param name="Id">The id of the user review.</param>
/// <param name="UserId">The id of the user.</param>
/// <param name="ProductId">The id of the product.</param>
/// <param name="Description">Description of the review.</param>
/// <param name="Score">The product score.</param>
///  <param name="DateAdded">The date of added review.</param>
public sealed record ProductReviewObject
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public string? Description { get; set; }
    public int Score { get; set; }
    public DateTime DateAdded { get; set; }

}
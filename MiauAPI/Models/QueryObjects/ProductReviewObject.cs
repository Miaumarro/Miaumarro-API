using MiauDatabase.Enums;

namespace MiauAPI.Models.QueryObjects;

/// <summary>
/// Represents the object in a product query.
/// </summary>
/// <param name="Id">The description of the product.</param>
/// <param name="Name">The name of the product.</param>
/// <param name="Description">The description of the product.</param>
/// <param name="Price">The price of the product.</param>
/// <param name="IsActive">Determines whether this product is available for sale.</param>
/// <param name="Amount">The product's amount in stock.</param>
/// <param name="Tags">The product's store tags.</param>
/// <param name="Brand">The product's brand.</param>
/// <param name="Discount">The password entered by the user.</param>
public sealed record ProductReviewObject
{
    public int Id { get; set; }
    public DateTime DateAdded { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Score { get; set; }
} 
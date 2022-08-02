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
public sealed record ProductObject
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? Brand { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public int Amount { get; set; }
    public ProductTag Tags { get; set; }
    public decimal Discount { get; set; }

} 
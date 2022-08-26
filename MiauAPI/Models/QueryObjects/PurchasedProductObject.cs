using MiauDatabase.Enums;

namespace MiauAPI.Models.QueryObjects;

/// <summary>
/// Represents the object in a purchased product query.
/// </summary>
/// <param name="Id">The id of the purchased product.</param>
/// <param name="ProductId">The id of the product the purchased product is related to.</param>
/// <param name="PurchaseId">The id of the purchase the purchased product is related to.</param>
/// <param name="SalePrice">The purchase price.</param>


public sealed record PurchasedProductObject
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public int PurchaseId { get; init; }
    public string SalePrice { get; init; } = null!;
} 
using MiauDatabase.Enums;

namespace MiauAPI.Models.QueryObjects;

/// <summary>
/// Represents the object in a purchase query.
/// </summary>
/// <param name="Id">The description of the purchase.</param>
/// <param name="UserId">The id of the user the purchase is related to.</param>
/// <param name="CouponId">The id of the coupon the purchase is related to.</param>
/// <param name="PurchasedProduct">The purchased product.</param>
/// <param name="Status">The purchase status.</param>


public sealed record PurchaseObject
{
    public int Id { get; init; }
    public int UserId { get; init; } 
    public int? CouponId { get; init; }
    public List<PurchasedProductObject>? PurchasedProduct { get; init; }
    public PurchaseStatus Status { get; init; }


} 
using MiauDatabase.Enums;

namespace MiauAPI.Models.QueryObjects;

/// <summary>
/// Represents the object in an Purchase query.
/// </summary>
/// <param name="Id">The id of the WishList.</param>
/// <param name="UserId">The id of the user the Purchase is related to.</param>
/// <param name="CouponId">The id of the coupon the Purchase is related to.</param>
/// <param name="Status">The status of purchase.</param>


public sealed record PurchaseObject
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public int CouponId { get; set; }
    public PurchaseStatus Status { get; set; }


}

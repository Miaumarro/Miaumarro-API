using MiauDatabase.Enums;

namespace MiauAPI.Models.QueryObjects;

/// <summary>
/// Represents a purchase object.
/// </summary>
/// <param name="Id">The Id of the purchase.</param>
/// <param name="Status">The current status of the purchase.</param>
/// <param name="DateAdded">The date and time the purchase was made.</param>
/// <param name="Products">The products purchased in the transaction.</param>
/// <param name="Coupon">The coupon used for the purchase.</param>
public sealed record PurchaseObject(int Id, PurchaseStatus Status, DateTime DateAdded, ProductObject[] Products, CouponObject? Coupon);

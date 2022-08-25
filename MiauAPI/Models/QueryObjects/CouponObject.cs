namespace MiauAPI.Models.QueryObjects;

/// <summary>
/// Represents a purchase coupon.
/// </summary>
/// <param name="Id">The Id of the coupon.</param>
/// <param name="Coupon">The coupon itself.</param>
/// <param name="IsActive">Whether the coupon is currently active or not.</param>
/// <param name="Discount">How much discount does the coupon apply.</param>
public sealed record CouponObject(int Id, string Coupon, bool IsActive, decimal Discount);

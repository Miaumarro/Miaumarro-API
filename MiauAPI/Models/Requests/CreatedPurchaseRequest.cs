using MiauDatabase.Enums;
namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for creation of a new purchase.
/// </summary>
/// <param name="UserId">The user this purchase is associated with.</param>
/// <param name="ProductsId">The Id of the purchased products this purchase is associated with.</param>
/// <param name="CouponId">The discount coupon used for this purchase.</param>
public sealed record CreatedPurchaseRequest(int UserId, int[] ProductIds, int? CouponId);
using MiauDatabase.Enums;

namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for creation of a new Purchase.
/// </summary>
/// <param name="Id">The id of the purchase.</param>
/// <param name="UserId">The id of the user the Purchase is related to.</param>
/// <param name="CouponId">The id of the coupon the Purchase is related to.</param>
/// <param name="Status">The status of purchase.</param>

public sealed record CreatedPurchaseRequest(int UserId, int CouponId, PurchaseStatus Status);
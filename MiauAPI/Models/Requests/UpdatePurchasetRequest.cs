using MiauDatabase.Enums;

namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for update a pet by a given Id.
/// </summary>
/// <param name="Id">The id of the purchase.</param>
/// <param name="UserId">The id of the user the Purchase is related to.</param>
/// <param name="CouponId">The id of the coupon the Purchase is related to.</param>
/// <param name="Status">The status of purchase.</param>


public sealed record UpdatePurchaseRequest(int UserId, int CouponId, int Status);

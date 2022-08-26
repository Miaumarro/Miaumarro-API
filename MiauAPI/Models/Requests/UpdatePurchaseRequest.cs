using MiauDatabase.Enums;

namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for update a review by a given Id.
/// </summary>
/// <param name="Id">The id of the user review.</param>
/// <param name="UserId">The id of the user.</param>
/// <param name="CouponId">The id of the product.</param>
/// <param name="Status">Description of the review.</param>

public sealed record UpdatePurchaseRequest(int Id, int UserId, int CouponId, PurchaseStatus Status);

using MiauDatabase.Enums;

namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for updating a purchase.
/// </summary>
/// <param name="Id">The Id o the purchase.</param>
/// <param name="UserId">The Id of the user who made the purchase.</param>
/// <param name="Status">The new status of the purchase.</param>
public sealed record UpdatePurchaseRequest(int Id, int UserId, PurchaseStatus Status);
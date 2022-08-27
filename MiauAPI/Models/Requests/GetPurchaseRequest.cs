namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for a single purchase.
/// </summary>
/// <param name="UserId">The user's Id.</param>
/// <param name="PurchaseId">The purchase's Id.</param>
public sealed record GetPurchaseRequest(int UserId, int PurchaseId);
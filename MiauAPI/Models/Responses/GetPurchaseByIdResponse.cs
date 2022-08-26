using MiauAPI.Models.QueryObjects;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response when a review is requested by its Id.
/// </summary>
/// <param name="Purchase">The resulted review.</param>
public sealed record GetPurchaseByIdResponse(PurchaseObject Purchase);

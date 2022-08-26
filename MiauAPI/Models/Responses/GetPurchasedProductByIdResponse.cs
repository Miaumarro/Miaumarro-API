using MiauAPI.Models.QueryObjects;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response when a review is requested by its Id.
/// </summary>
/// <param name="PurchasedProduct">The resulted review.</param>
public sealed record GetPurchasedProductByIdResponse(PurchasedProductObject PurchasedProduct);

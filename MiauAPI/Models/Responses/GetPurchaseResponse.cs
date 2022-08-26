using MiauAPI.Models.QueryObjects;
using MiauAPI.Pagination;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a product query is successfully executed.
/// </summary>
/// <param name="Purchases">The resulted list of products.</param>
public sealed record GetPurchaseResponse(PagedList<PurchaseObject> Purchases);
using MiauAPI.Models.QueryObjects;
using MiauAPI.Pagination;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a wish list query is successfully executed.
/// </summary>
/// <param name="Purchase">The resulted list of wish products.</param>
public sealed record GetPurchaseResponse(PagedList<PurchaseObject> Purchase);

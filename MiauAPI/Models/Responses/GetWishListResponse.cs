using MiauAPI.Models.QueryObjects;
using MiauAPI.Pagination;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a wish list query is successfully executed.
/// </summary>
/// <param name="WishLiests">The resulted list of wish products.</param>
public sealed record GetWishListResponse(PagedList<WishListObject> WishList);

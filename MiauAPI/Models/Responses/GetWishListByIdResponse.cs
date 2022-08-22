using MiauAPI.Models.QueryObjects;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response when a wish List is requested by its Id.
/// </summary>
/// <param name="wishList">The resulted wish list.</param>
public sealed record GetWishListByIdResponse(WishListObject WishList);

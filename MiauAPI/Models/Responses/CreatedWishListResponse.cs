namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a item is successfully added in the wishlist.
/// </summary>
/// <param name="Id">The database ID of the wish list.</param>
public sealed record CreatedWishlistResponse(int Id);
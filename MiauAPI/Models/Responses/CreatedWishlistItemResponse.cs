namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a item is successfully added in the wishlist.
/// </summary>
/// <param name="Id">The database ID of the wishlist item.</param>
public sealed record CreatedWishlistItemResponse(int Id);
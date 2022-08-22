namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request to add a new product to a user's wish list.
/// </summary>
/// <param name="UserId">The user this wishlist item is associated with.</param>
/// <param name="ProductId">The product this wishlist item is associated with.</param>
public sealed record CreatedWishlistItemRequest(int UserId, int ProductId);
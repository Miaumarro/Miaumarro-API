namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for deleting a wishlist item.
/// </summary>
/// <param name="UserId">The user's Id.</param>
/// <param name="ProductId">The product's Id.</param>
public sealed record DeleteWishlistRequest(int UserId, int ProductId);
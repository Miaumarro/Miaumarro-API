using MiauDatabase.Enums;

namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for creation of a new WishList.
/// </summary>
/// <param name="UserId">The id of the user.</param>
/// <param name="ProductId">The id of the product.</param>

public sealed record CreatedWishlistRequest(int UserId, int ProductId);
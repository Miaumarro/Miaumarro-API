using MiauDatabase.Enums;

namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for update a pet by a given Id.
/// </summary>
/// <param name="Id">The id of the WishList.</param>
/// <param name="UserId">The id of the user.</param>
/// <param name="ProductId">The id of the product.</param>

public sealed record UpdateWishListRequest(int Id, int UserId, int ProductId);

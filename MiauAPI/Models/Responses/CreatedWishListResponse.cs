namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a wish list is successfully created.
/// </summary>
/// <param name="Id">The database ID of the wish list.</param>
public sealed record CreatedWishListResponse(int Id);
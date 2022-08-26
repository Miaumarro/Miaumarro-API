using MiauDatabase.Enums;

namespace MiauAPI.Models.QueryObjects;

/// <summary>
/// Represents the object in an WishList query.
/// </summary>
/// <param name="Id">The id of the WishList.</param>
/// <param name="UserId">The id of the user the WishList is related to.</param>
/// <param name="ProductId">The id of the product the WishList is related to.</param>

public sealed record WishlistObject
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public int ProductId { get; set; }
 

}

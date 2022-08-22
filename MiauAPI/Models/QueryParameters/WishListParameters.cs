namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Set relevant parameters for an WishList search.
/// </summary>
/// <param name="UserId">The id of the user related to the wish list.</param>
public sealed class WishListParameters : QueryStringParameters
{
    public int UserId { get; init; }

}


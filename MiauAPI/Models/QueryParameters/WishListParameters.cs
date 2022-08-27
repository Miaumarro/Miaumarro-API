namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Set relevant parameters for an WishList search.
/// </summary>
/// <param name="UserId">The Id of the user related to the wish list.</param>
public sealed record WishlistParameters(int UserId) : QueryStringParameters;
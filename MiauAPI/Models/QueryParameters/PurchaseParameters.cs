namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Set relevant parameters for a purchase search.
/// </summary>
/// <param name="UserId">The id of the user related to the purchase.</param>
public sealed class PurchaseParameters : QueryStringParameters
{
    public int UserId { get; init; }
}


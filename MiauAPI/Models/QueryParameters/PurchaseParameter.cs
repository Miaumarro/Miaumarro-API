namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Set relevant parameters for a purchase search.
/// </summary>
/// <param name="UserId">The id of the user the purchase is related to.</param>

public sealed class PurchaseParameters : QueryStringParameters
{
    public int UserId { get; init; }



}


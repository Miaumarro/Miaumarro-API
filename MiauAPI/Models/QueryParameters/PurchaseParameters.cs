namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Query parameters for purchases.
/// </summary>
/// <param name="UserId">The Id of the user who made the purchases.</param>
public sealed record PurchaseParameters(int UserId) : QueryStringParameters;
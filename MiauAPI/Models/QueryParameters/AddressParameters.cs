namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Set relevant parameters for a adress search.
/// </summary>
/// <param name="UserId">The id of the user related to the address.</param>
public sealed record AddressParameters(int UserId) : QueryStringParameters;
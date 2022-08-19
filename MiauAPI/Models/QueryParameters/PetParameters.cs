namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Set relevant parameters for a pet search.
/// </summary>
/// <param name="UserId">The id of the user related to the pet.</param>
public sealed record PetParameters(int UserId) : QueryStringParameters;
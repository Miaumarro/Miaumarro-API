using MiauAPI.Enums;
using MiauDatabase.Enums;

namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Set relevant parameters for a pet search.
/// </summary>
/// <param name="UserId">The id of the user related to the pet.</param>
public sealed class PetParameters : QueryStringParameters
{
    public int UserId { get; init; }

}


namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Set relevant parameters for an user search.
/// </summary>
/// <param name="Cpf">The cpf of the user.</param>
public sealed class UserParameters : QueryStringParameters
{
    public string? Cpf { get; init; }
}


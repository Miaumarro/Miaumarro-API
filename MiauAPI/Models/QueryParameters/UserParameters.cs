namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Set relevant parameters for an user search.
/// </summary>
/// <param name="Ids">The user Ids to search for.</param>
/// <param name="Cpfs">The user Cpfs to search for.</param>
/// <param name="Emails">The user e-mails to search for.</param>
public sealed record UserParameters(int[] Ids, string[] Cpfs, string[] Emails) : QueryStringParameters
{
    public UserParameters() : this(Array.Empty<int>(), Array.Empty<string>(), Array.Empty<string>())
    {
    }
}
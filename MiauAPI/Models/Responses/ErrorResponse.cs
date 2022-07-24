namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents a generic error response.
/// </summary>
/// <param name="Errors">The errors to be sent in the response.</param>
public sealed record ErrorResponse(string[] Errors)
{
    public ErrorResponse(string message) : this(new string[] { message })
    {
    }
}

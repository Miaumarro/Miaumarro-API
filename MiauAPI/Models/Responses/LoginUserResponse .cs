namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a user is successfully created.
/// </summary>
/// <param name="SessionToken">The session token of the user.</param>
public sealed record LoginUserResponse(string? SessionToken);
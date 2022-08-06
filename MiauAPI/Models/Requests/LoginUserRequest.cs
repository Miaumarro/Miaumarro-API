namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for creation of a new user.
/// </summary>
/// <param name="Email">The e-mail of the user.</param>
/// <param name="Password">The password entered by the user.</param>
public sealed record LoginUserRequest(string Email, string Password);
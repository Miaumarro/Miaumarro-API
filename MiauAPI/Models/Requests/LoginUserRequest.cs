namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for user authentication.
/// </summary>
/// <param name="Cpf">The CPF of the user.</param>
/// <param name="Email">The e-mail of the user.</param>
/// <param name="Password">The password of the user.</param>
public sealed record LoginUserRequest(string? Cpf, string? Email, string Password);
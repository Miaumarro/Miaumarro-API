namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for creation of a new user.
/// </summary>
/// <param name="Cpf">The CPF of the user.</param>
/// <param name="Name">The name of the user.</param>
/// <param name="Surname">The surname of the user.</param>
/// <param name="Email">The e-mail of the user.</param>
/// <param name="Phone">The phone number of the user.</param>
/// <param name="Password">The password entered by the user.</param>
public sealed record CreatedUserRequest(string Cpf, string Name, string Surname, string Email, string? Phone, string Password);
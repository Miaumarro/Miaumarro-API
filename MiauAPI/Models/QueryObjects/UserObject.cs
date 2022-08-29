namespace MiauAPI.Models.QueryObjects;

/// <summary>
/// Represents the object in an user query.
/// </summary>
/// <param name="Id">The Id of the user.</param>
/// <param name="Cpf">The CPF of the user.</param>
/// <param name="Name">The name of the user.</param>
/// <param name="Surname">The surname of the user.</param>
/// <param name="Email">The e-mail of the user.</param>
/// <param name="Phone">The phone number of the user.</param>
/// <param name="Password">The password entered by the user.</param>
public sealed record UserObject(int Id, string Cpf, string Name, string Surname, string Email, string? Phone);
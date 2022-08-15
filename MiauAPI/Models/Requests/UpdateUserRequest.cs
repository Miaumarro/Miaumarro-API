namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for update an user by a given Id.
/// </summary>
/// <param name="Id">The id of the user.</param>
/// <param name="Name">The name of the user.</param>
/// <param name="Surname">The surname of the user.</param>
/// <param name="Email">The e-mail of the user.</param>
/// <param name="Phone">The phone number of the user.</param>
public sealed record UpdateUserRequest(int Id, string Name, string Surname, string Email, string? Phone);
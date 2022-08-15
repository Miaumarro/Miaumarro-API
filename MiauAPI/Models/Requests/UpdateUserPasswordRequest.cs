namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for update an user password by a given Id.
/// </summary>
/// <param name="Id">The id of the user.</param>
/// <param name="Password">The password entered by the user.</param>
public sealed record UpdateUserPasswordRequest(int Id, string Password);
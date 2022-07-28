namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a user is successfully created.
/// </summary>
/// <param name="Id">The database ID of the pet.</param>
public sealed record CreatedPetResponse(int Id);
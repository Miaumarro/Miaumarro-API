using MiauDatabase.Enums;

namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for update a pet by a given Id.
/// </summary>
/// <param name="Id">The id of the pet.</param>
/// <param name="UserId">The id of the pet's user.</param>
/// <param name="Name">The name of the pet.</param>
/// <param name="Type">The type of the pet.</param>
/// <param name="Gender">The gender of the pet.</param>
/// <param name="Breed">The breed of the pet.</param>
/// <param name="Image">An image of the pet.</param>
/// <param name="DateOfBirth">This pet's date of birth.</param>
public sealed record UpdatePetRequest(int Id, int UserId, string Name, PetType Type, PetGender Gender, string? Breed, byte[]? Image, DateTime DateOfBirth);

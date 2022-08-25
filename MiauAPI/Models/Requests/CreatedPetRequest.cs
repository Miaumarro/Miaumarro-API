using MiauDatabase.Enums;

namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for creation of a new pet.
/// </summary>
/// <param name="UserId">The id of pet's user.</param>
/// <param name="Name">The name of the pet.</param>
/// <param name="Type">The type of the pet.</param>
/// <param name="Gender">The gender of the pet.</param>
/// <param name="DateOfBirth">This pet's date of birth.</param>
/// <param name="Breed">The breed of the pet.</param>
/// <param name="Image">An image of the pet.</param>
public sealed record CreatedPetRequest(int UserId, string Name, PetType Type, PetGender Gender, DateTime DateOfBirth, string? Breed, byte[]? Image);
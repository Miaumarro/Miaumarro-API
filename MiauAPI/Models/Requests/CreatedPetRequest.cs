using MiauDatabase.Entities;
using MiauDatabase.Enums;

namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for creation of a new pet.
/// </summary>
/// <param name="User">The pet's user.</param>
/// <param name="Name">The name of the pet.</param>
/// <param name="Type">The type of the pet.</param>
/// <param name="Gender">The gender of the pet.</param>
/// <param name="Breed">The breed of the pet.</param>
/// <param name="ImageFileUrl">This pet's image location in the file system.</param>
/// <param name="DateOfBirth">This pet's date of birth.</param>
public sealed record CreatedPetRequest(UserEntity User, string Name, PetType Type, PetGender Gender, string? Breed, string? ImageFileUrl, DateTime DateOfBirth);
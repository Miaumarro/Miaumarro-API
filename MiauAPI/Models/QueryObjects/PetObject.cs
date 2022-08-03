using MiauDatabase.Enums;

namespace MiauAPI.Models.QueryObjects;

/// <summary>
/// Represents the object in a pet query.
/// </summary>
/// <param name="Id">The id of the pet.</param>
/// <param name="UserId">The id of the user the pet is related to.</param>
/// <param name="Name">The name of the pet.</param>
/// <param name="Type">The type of the pet.</param>
/// <param name="Gender">The gender of the pet.</param>
/// <param name="Breed">The breed of the pet.</param>
/// <param name="ImagePath">The image path to the pet's image.</param>
/// <param name="DateOfBirth">The date of birth of the pet.</param>
public sealed record PetObject
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string Name { get; set; } = null!;
    public PetType Type { get; set; }
    public PetGender Gender { get; set; }
    public string Breed { get; set; } = null!;
    public string ImagePath { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }

}

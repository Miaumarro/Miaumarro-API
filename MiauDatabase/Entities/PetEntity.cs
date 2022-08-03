using MiauDatabase.Abstractions;
using MiauDatabase.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MiauDatabase.Entities;

/// <summary>
/// Represents a database pet.
/// </summary>
[Comment("Represents a pet.")]
public sealed record PetEntity : MiauDbEntity
{
    /// <summary>
    /// The user this pet is associated with.
    /// </summary>
    public UserEntity User { get; init; } = null!;

    /// <summary>
    /// The appointments associated with this pet.
    /// </summary>
    public List<AppointmentEntity> Appointments { get; init; } = new();

    /// <summary>
    /// This pet's name.
    /// </summary>
    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// This pet's type.
    /// </summary>
    public PetType Type { get; init; }

    /// <summary>
    /// This pet's gender.
    /// </summary>
    public PetGender Gender { get; init; }

    /// <summary>
    /// This pet's breed.
    /// </summary>
    [MaxLength(30)]
    public string? Breed { get; init; }

    /// <summary>
    /// This pet's image location in the file system.
    /// </summary>
    [MaxLength(256)]
    public string? ImagePath { get; set; }

    /// <summary>
    /// This pet's date of birth.
    /// </summary>
    public DateTime DateOfBirth { get; init; }
}
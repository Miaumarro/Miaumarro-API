using MiauDatabase.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MiauDatabase.Entities;

/// <summary>
/// Represents a database address.
/// </summary>
[Comment("Represents an address.")]
public sealed record AddressEntity : MiauDbEntity
{
    private string _cep = null!;

    /// <summary>
    /// The user this address is associated with.
    /// </summary>
    public UserEntity User { get; init; } = null!;

    /// <summary>
    /// The body of this address (street name).
    /// </summary>
    [Required]
    [MaxLength(60)]
    public string Address { get; init; } = null!;

    /// <summary>
    /// The number of this address.
    /// </summary>
    public int Number { get; init; }

    /// <summary>
    /// The geographical reference for this address.
    /// </summary>
    [MaxLength(100)]
    public string? Reference { get; init; }

    /// <summary>
    /// The complement for this address.
    /// </summary>
    [MaxLength(15)]
    public string? Complement { get; init; }

    /// <summary>
    /// The neighborhood this address is located in.
    /// </summary>
    [Required]
    [MaxLength(30)]
    public string Neighborhood { get; init; } = null!;

    /// <summary>
    /// The city this address is located in.
    /// </summary>
    [Required]
    [MaxLength(30)]
    public string City { get; init; } = null!;

    /// <summary>
    /// The state this address is located in.
    /// </summary>
    [Required]
    [MaxLength(30)]
    public string State { get; init; } = null!;

    /// <summary>
    /// The name of the person tasked with receiving parcels for this address.
    /// </summary>
    [MaxLength(60)]
    public string? Destinatary { get; init; }

    /// <summary>
    /// The CEP of this address.
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string Cep
    {
        get => _cep;
        init => _cep = (value.Any(letter => !char.IsNumber(letter))) ? throw new ArgumentException("CEP can only contain numbers.", nameof(value)) : value;
    }
}
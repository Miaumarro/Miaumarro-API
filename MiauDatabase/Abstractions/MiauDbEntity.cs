using System.ComponentModel.DataAnnotations;

namespace MiauDatabase.Abstractions;

/// <summary>
/// Represents a database table.
/// </summary>
public abstract record MiauDbEntity
{
    /// <summary>
    /// The primary key of this entity.
    /// </summary>
    [Key]
    public int Id { get; init; }

    /// <summary>
    /// Date and time of when this entity was added to the database.
    /// </summary>
    public DateTime DateAdded { get; init; } = DateTime.UtcNow;
}
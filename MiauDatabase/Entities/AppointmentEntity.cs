using MiauDatabase.Abstractions;
using MiauDatabase.Enums;
using Microsoft.EntityFrameworkCore;

namespace MiauDatabase.Entities;

/// <summary>
/// Represents a database appointment.
/// </summary>
/// <value></value>
[Comment("Represents an appointment.")]
public sealed record AppointmentEntity : MiauDbEntity
{
    /// <summary>
    /// The pet this appointment is associated with.
    /// </summary>
    public PetEntity PetRel { get; init; } = null!;

    /// <summary>
    /// The ID of the pet associated with this appointment.
    /// </summary>
    public int PetIdFK { get; init; }

    /// <summary>
    /// How much the user is going to be charged for the service.
    /// </summary>
    public decimal Price { get; init; }

    /// <summary>
    /// The type of appointment that was scheduled.
    /// </summary>
    public AppointmentType Type { get; init; }

    /// <summary>
    /// The time the appointment is supposed to take place.
    /// </summary>
    public DateTime ScheduledTime { get; set; }
}
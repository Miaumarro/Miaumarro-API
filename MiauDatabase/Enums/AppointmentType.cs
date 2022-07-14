namespace MiauDatabase.Enums;

/// <summary>
/// Represents the kind of services available for appointment.
/// </summary>
[Flags]
public enum AppointmentType
{
    /// <summary>
    /// Grooming service.
    /// </summary>
    Grooming = 1 << 0,

    /// <summary>
    /// Bath service.
    /// </summary>
    Bath = 1 << 1
}
namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for a single appointment.
/// </summary>
/// <param name="PetId">The pet Id.</param>
/// <param name="AppointmentId">The appointment Id.</param>
public sealed record GetAppointmentRequest(int PetId, int AppointmentId);
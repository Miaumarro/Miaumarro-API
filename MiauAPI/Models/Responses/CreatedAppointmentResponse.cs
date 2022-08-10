namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a appointment is successfully created.
/// </summary>
/// <param name="Id">The database ID of the appointment.</param>
public sealed record CreatedAppointmentResponse(int Id);

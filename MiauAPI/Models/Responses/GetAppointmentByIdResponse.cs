using MiauAPI.Models.QueryObjects;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response when an appointment is requested by its Id.
/// </summary>
/// <param name="Appointment">The resulted appointment.</param>
public sealed record GetAppointmentByIdResponse(AppointmentObject Appointment);

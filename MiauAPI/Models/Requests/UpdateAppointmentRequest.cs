using MiauDatabase.Enums;

namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for update an appointment by a given Id.
/// </summary>
/// <param name="Id">The id of the appointment.</param>
/// <param name="PetId">The Id of the pet which the appointment is related to.</param>
/// <param name="Price">The Price of the appointment.</param>
/// <param name="Type">The Type of the appointment.</param>
/// <param name="ScheduledTime">The Scheduled Time of the appointment.</param>
public sealed record UpdateAppointmentRequest(int Id, int PetId, decimal Price, AppointmentType Type, DateTime ScheduledTime);

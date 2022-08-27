using MiauDatabase.Enums;

namespace MiauAPI.Models.QueryObjects;

/// <summary>
/// Represents the object in a appointment query.
/// </summary>
/// <param name="Id">The Id of the appointment.</param>
/// <param name="UserId">The Id of the user which the appointment is related to.</param>
/// <param name="PetId">The Id of the pet which the appointment is related to.</param>
/// <param name="Price">The Price of the appointment.</param>
/// <param name="Type">The Type of the appointment.</param>
/// <param name="ScheduledTime">The Scheduled Time of the appointment.</param>
public sealed record AppointmentObject(int Id, int UserId, int PetId, decimal Price, AppointmentType Type, DateTime ScheduledTime);
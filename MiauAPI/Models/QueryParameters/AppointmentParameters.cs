namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Set relevant parameters for a appointment search.
/// </summary>
/// <param name="UserId">The id of the user related to the appointment.</param>
/// <param name="PetId">The id of the pet related to the appointment.</param>
public sealed record AppointmentParameters(int UserId, int PetId) : QueryStringParameters;
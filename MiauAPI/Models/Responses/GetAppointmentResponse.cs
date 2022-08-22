using MiauAPI.Models.QueryObjects;
using MiauAPI.Pagination;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when aan appointment query is successfully executed.
/// </summary>
/// <param name="Appointments">The resulted list of appointments.</param>
public sealed record GetAppointmentResponse(PagedList<AppointmentObject> Appointments);

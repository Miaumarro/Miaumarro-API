using MiauAPI.Models.QueryObjects;
using MiauAPI.Pagination;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a pet query is successfully executed.
/// </summary>
/// <param name="Pets">The resulted list of pets.</param>
public sealed record GetPetResponse(PagedList<PetObject> Pets);

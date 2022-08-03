using MiauAPI.Models.QueryObjects;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response when a pet is requested by its Id.
/// </summary>
/// <param name="Pet">The resulted pet.</param>
public sealed record GetPetByIdResponse(PetObject Pet);

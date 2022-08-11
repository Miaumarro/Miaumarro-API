using MiauAPI.Models.QueryObjects;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response when an address is requested by its Id.
/// </summary>
/// <param name="Address">The resulted address.</param>
public sealed record GetAddressByIdResponse(AddressObject Address);

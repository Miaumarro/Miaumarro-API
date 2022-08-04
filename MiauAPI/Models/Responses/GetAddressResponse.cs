using MiauAPI.Models.QueryObjects;
using MiauAPI.Pagination;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a address query is successfully executed.
/// </summary>
/// <param name="Addresses">The resulted list of address.</param>
public sealed record GetAddressResponse(PagedList<AddressObject> Addresses);

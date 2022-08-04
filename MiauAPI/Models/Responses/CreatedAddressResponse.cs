namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a address is successfully created.
/// </summary>
/// <param name="Id">The database ID of the address.</param>
public sealed record CreatedAddressResponse(int Id);
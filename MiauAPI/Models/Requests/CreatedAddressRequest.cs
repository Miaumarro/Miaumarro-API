namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for creation of a new address.
/// </summary>
/// <param name="UserId">The user this address is associated with.</param>
/// <param name="State">The state this address is located in.</param>
/// <param name="City">The city this address is located in.</param>
/// <param name="Neighborhood">The neighborhood this address is located in.</param>
/// <param name="Cep">The CEP of this address.</param>
/// <param name="Address">The body of this address (street name).</param>
/// <param name="AddressNumber">The number of this address.</param>
/// <param name="Complement">The complement for this address.</param>
/// <param name="Reference">The geographical reference for this address.</param>
/// <param name="Destinatary">The name of the person tasked with receiving parcels for this address.</param>
public sealed record CreatedAddressRequest(int UserId, string State, string City, string Neighborhood, string Cep, string Address, int AddressNumber, string? Complement, string? Reference, string? Destinatary);

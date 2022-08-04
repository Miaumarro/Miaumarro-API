namespace MiauAPI.Models.QueryObjects;

/// <summary>
/// Represents the object in a address query.
/// </summary>
/// <param name="Id">The id of the address.</param>
/// <param name="UserId">The user this address is associated with.</param>
/// <param name="Address">The body of this address (street name).</param>
/// <param name="Number">The number of this address.</param>
/// <param name="Reference">The geographical reference for this address.</param>
/// <param name="Complement">The complement for this address.</param>
/// <param name="Neighborhood">The neighborhood this address is located in.</param>
/// <param name="City">The city this address is located in.</param>
/// <param name="State">The state this address is located in.</param>
/// <param name="Destinatary">The name of the person tasked with receiving parcels for this address.</param>
/// <param name="Cep">The CEP of this address.</param>
public sealed record AddressObject
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string Address { get; set; } = null!;
    public int Number { get; set; }
    public string? Reference { get; set; }
    public string? Complement { get; set; }
    public string Neighborhood { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string? Destinatary { get; set; }
    public string Cep { get; set; } = null!;

}




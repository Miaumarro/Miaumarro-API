namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a purchase is successfully created.
/// </summary>
/// <param name="Id">The database ID of the purchase.</param>
public sealed record CreatedPurchaseResponse(int Id);

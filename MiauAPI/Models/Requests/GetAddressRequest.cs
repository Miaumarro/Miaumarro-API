namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request to get one address.
/// </summary>
/// <param name="UserId">The user's Id.</param>
/// <param name="AddressId">The address' Id.</param>
public sealed record GetAddressRequest(int UserId, int AddressId);
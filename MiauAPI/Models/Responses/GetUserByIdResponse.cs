using MiauAPI.Models.QueryObjects;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response when an user is requested by its Id.
/// </summary>
/// <param name="User">The resulted appointment.</param>
public sealed record GetUserByIdResponse(UserObject User);

namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for updating a user password.
/// </summary>
/// <param name="Id">The id of the user.</param>
/// <param name="OldPassword">The old password.</param>
/// <param name="NewPassword">The new password.</param>
public sealed record UpdateUserPasswordRequest(int Id, string OldPassword, string NewPassword);
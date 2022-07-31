namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents a generic response for a successful update action.
/// </summary>
/// <param name="Message">The message to be sent in the response.</param>
public sealed record UpdateResponse(params string[] Message);
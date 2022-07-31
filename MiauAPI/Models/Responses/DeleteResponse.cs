namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents a generic response for a successful delete action.
/// </summary>
/// <param name="Errors">The errors to be sent in the response.</param>
public sealed record DeleteResponse(params string[] Message);
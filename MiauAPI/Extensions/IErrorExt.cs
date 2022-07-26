using FluentResults;
using MiauAPI.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MiauAPI.Extensions;

/// <summary>
/// Contains extension methods for <see cref="IError"/>.
/// </summary>
public static class IErrorExt
{
    /// <summary>
    /// Key for metadata error responses.
    /// </summary>
    private const string _identifierKey = "HTTPStatusCode";

    /// <summary>
    /// Maps some special success response objects to HTTP status codes.
    /// </summary>
    private static readonly IReadOnlyDictionary<int, Func<string, object?, IActionResult>> _successCodeObjects = new Dictionary<int, Func<string, object?, IActionResult>>()
    {
        [StatusCodes.Status200OK] = (message, response) => (message is null && response is null) ? new OkResult() : new OkObjectResult(response ?? message),
        [StatusCodes.Status201Created] = (location, response) => new CreatedResult(location, response),
        [StatusCodes.Status202Accepted] = (location, response) => new AcceptedResult(location, response),
        [StatusCodes.Status204NoContent] = (_, _) => new NoContentResult(),
    };

    /// <summary>
    /// Maps some special error response objects to HTTP status codes.
    /// </summary>
    private static readonly IReadOnlyDictionary<int, Func<string, ObjectResult>> _errorCodeObjects = new Dictionary<int, Func<string, ObjectResult>>()
    {
        [StatusCodes.Status400BadRequest] = message => new BadRequestObjectResult(new ErrorResponse(message)),
        [StatusCodes.Status401Unauthorized] = message => new UnauthorizedObjectResult(new ErrorResponse(message)),
        [StatusCodes.Status404NotFound] = message => new NotFoundObjectResult(new ErrorResponse(message)),
        [StatusCodes.Status409Conflict] = message => new ConflictObjectResult(new ErrorResponse(message)),
        [StatusCodes.Status422UnprocessableEntity] = message => new UnprocessableEntityObjectResult(new ErrorResponse(message)),
    };

    /// <summary>
    /// Adds or updates an <see cref="ObjectResult"/> that represents an error code
    /// to the metadata of this error.
    /// </summary>
    /// <param name="error">This error.</param>
    /// <param name="statusCode">The HTTP status code to be set.</param>
    /// <returns>This error with the new code set.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="statusCode"/> is not a HTTP error code.</exception>
    public static IError WithCode(this IError error, int statusCode)
    {
        if (statusCode is < 400 or > 599)
            throw new ArgumentException($"'{statusCode}' is not an HTTP error code.", nameof(statusCode));

        error.Metadata.Remove(_identifierKey);

        error.Metadata.TryAdd(
            _identifierKey,
            (_errorCodeObjects.TryGetValue(statusCode, out var resolver))
                ? resolver(error.Message)
                : new ObjectResult(new ErrorResponse(error.Message)) { StatusCode = statusCode }
        );

        return error;
    }

    public static ISuccess WithCode<T>(this ISuccess success, int statusCode, T? response, string? location = default)
    {
        if (statusCode is < 200 or > 299)
            throw new ArgumentException($"'{statusCode}' is not an HTTP success code.", nameof(statusCode));
        else if (statusCode is 201 or 202 && string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location must be present for status code 201 and 202", nameof(location));

        success.Metadata.Remove(_identifierKey);

        success.Metadata.TryAdd(
            _identifierKey,
            (_successCodeObjects.TryGetValue(statusCode, out var resolver))
                ? resolver(location!, response)
                : new ObjectResult(response) { StatusCode = statusCode }
        );

        return success;
    }
}

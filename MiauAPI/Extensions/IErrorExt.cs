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
    private const string _identifierKey = "HTTPErrorCode";

    /// <summary>
    /// Maps some special response objects to HTTP status codes.
    /// </summary>
    private static readonly IReadOnlyDictionary<int, Func<string, ObjectResult>> _codeObjects = new Dictionary<int, Func<string, ObjectResult>>()
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
            (_codeObjects.TryGetValue(statusCode, out var resolver))
                ? resolver(error.Message)
                : new ObjectResult(new ErrorResponse(error.Message)) { StatusCode = statusCode }
        );

        return error;
    }
}

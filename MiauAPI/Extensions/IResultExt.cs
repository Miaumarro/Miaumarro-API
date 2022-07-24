using FluentResults;
using MiauAPI.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace MiauAPI.Extensions;

/// <summary>
/// Extension methods for <see cref="IResult{TValue}"/>.
/// </summary>
public static class IResultExt
{
    /// <summary>
    /// Converts this <see cref="IResult"/> into an <see cref="IActionResult"/>.
    /// </summary>
    /// <param name="result">This result.</param>
    /// <typeparam name="T">The type of the response.</typeparam>
    /// <returns>This result as an <see cref="IActionResult"/>.</returns>
    public static IActionResult ToActionResult<T>(this IResult<T> result)
    {
        // Attempts to use the ObjectResult stored in the IResult's metadata
        // If there isn't any, return default responses (200 for success, 500 for failure)
        return (result.HasActionResult(out var actionResult))
            ? actionResult
            : (result.IsSuccess)
                ? new OkObjectResult(result.Value)
                : new ObjectResult(new ErrorResponse("No valid response could be found for this request.")) { StatusCode = StatusCodes.Status500InternalServerError };
    }

    /// <summary>
    /// Checks if this <see cref="IResult{TValue}"/> contains an <see cref="IActionResult"/> in its metadata.
    /// </summary>
    /// <param name="result">This result.</param>
    /// <param name="actionResult">The first <see cref="IActionResult"/> found in the metadata or <see langword="null"/> if none is found.</param>
    /// <typeparam name="T">The type of the response.</typeparam>
    /// <returns><see langword="true"/> if an <see cref="IActionResult"/> was found, <see langword="false"/> otherwise.</returns>
    public static bool HasActionResult<T>(this IResult<T> result, [MaybeNullWhen(false)] out IActionResult actionResult)
    {
        var metaCollection = (result.IsSuccess)
            ? result.Successes.SelectMany(x => x.Metadata.Values).OfType<ObjectResult>()
            : result.Errors.SelectMany(x => x.Metadata.Values).OfType<ObjectResult>();

        var first = metaCollection.FirstOrDefault();

        if (first is null)
        {
            actionResult = default;
            return false;
        }

        // Grab all responses in the metadata and put then in the response body.
        // Success - need to be put directly into the body if there is only one response or into an array if there are multiple responses.
        // Failure - errors need to be inside an object that contains an array called "errors", so we have to grab all errors
        // in the metadata and merge them together into a single array.
        first.Value = (metaCollection.Count() is 1)
            ? first.Value
            : (result.IsSuccess)
                ? metaCollection.Select(x => x.Value).ToArray()
                : new ErrorResponse(
                    metaCollection
                        .Select(x => x.Value)
                        .OfType<ErrorResponse>()
                        .SelectMany(x => x.Errors)
                        .ToArray()
                );

        actionResult = first;
        return true;
    }
}
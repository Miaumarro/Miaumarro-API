using FluentResults;
using MiauAPI.Extensions;

namespace MiauAPI.Common;

public static class Respond
{
    public static Result<T> Ok<T>(T response, int statusCode = StatusCodes.Status200OK, string? location = default)
        => Result.Ok(response).WithSuccess(new Success(response?.ToString() ?? string.Empty).WithCode(statusCode, response, location));

    public static Result Fail(string response, int statusCode = StatusCodes.Status400BadRequest)
        => Result.Fail(response).WithError(new Error(response).WithCode(statusCode));

    public static Result Fail(IEnumerable<IError> errors)
        => Result.Fail(errors);

    public static Result Fail(IEnumerable<string> errorMessages, int statusCode = StatusCodes.Status400BadRequest)
        => Result.Fail(errorMessages.Select(message => new Error(message).WithCode(statusCode)));
}
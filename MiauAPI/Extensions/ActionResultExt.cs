using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace MiauAPI.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ActionResult"/>.
/// </summary>
public static class ActionResultExt
{
    /// <summary>
    /// Attempts to extract a response of type <typeparamref name="T"/> from this <see cref="ActionResult"/>.
    /// </summary>
    /// <typeparam name="T">The expected type of the response.</typeparam>
    /// <returns><see langword="true"/> is the response was unwrapped successfully, <see langword="false"/> otherwise.</returns>
    public static bool TryUnwrap<T>(this ActionResult? actionResult, [MaybeNullWhen(false)] out T response) where T : class
    {
        response = (actionResult is ObjectResult objectResult && objectResult.Value is T responseObject)
            ? responseObject
            : default;

        return response is not default(T);
    }
}
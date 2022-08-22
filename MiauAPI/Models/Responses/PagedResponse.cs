
using System.Collections;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Contains the metadata to be returned for paginated responses.
/// </summary>
/// <param name="PageNumber">The number of the current page.</param>
/// <param name="PageSize">The amount of results to be returned for each page.</param>
/// <param name="PreviousCount">The amount of results on previous pages.</param>
/// <param name="NextCount">The amount of results on the next pages.</param>
/// <param name="Amount">The amount of results in the current response.</param>
public abstract record PagedResponse(int PageNumber, int PageSize, int PreviousCount, int NextCount, int Amount)
{
    /// <summary>
    /// Creates a <see cref="PagedResponse{T}"/> according to the type of <paramref name="response"/>.
    /// </summary>
    /// <param name="pageNumber">The number of the current page.</param>
    /// <param name="pageSize">The amount of results to be returned for each page.</param>
    /// <param name="previousCount">The amount of results on previous pages.</param>
    /// <param name="nextCount">The amount of results on the next pages.</param>
    /// <param name="amount">The amount of results in the current response.</param>
    /// <param name="response">A collection with the results of the operation.</param>
    /// <typeparam name="T">The type of the returned collection.</typeparam>
    /// <returns>A <see cref="PagedResponse{T}"/> with results in a <typeparamref name="T"/>.</returns>
    public static PagedResponse<T> Create<T>(int pageNumber, int pageSize, int previousCount, int nextCount, int amount, T response) where T : ICollection
        => new(pageNumber, pageSize, previousCount, nextCount, amount, response);
}

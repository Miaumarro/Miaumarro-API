using System.Collections;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Contains the metadata to be returned on paginated responses and the result.
/// </summary>
public sealed record PagedResponse<T> : PagedResponse where T : ICollection
{
    /// <summary>
    /// A collection with the results of the operation.
    /// </summary>
    public T Response { get; init; }

    public PagedResponse(int pageNumber, int pageSize, int previousCount, int nextCount, int amount, T response) : base(pageNumber, pageSize, previousCount, nextCount, amount)
        => Response = response;
}
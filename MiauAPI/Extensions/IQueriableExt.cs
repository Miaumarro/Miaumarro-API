namespace MiauAPI.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IQueryable{T}"/>.
/// </summary>
public static class IQueriableExt
{
    /// <summary>
    /// Gets the range of the query in a page format.
    /// </summary>
    /// <param name="query">This query.</param>
    /// <param name="pageNumber">The number of the page.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <remarks>If <paramref name="pageSize"/> is 0, this query is ignored.</remarks>
    /// <returns>This query with the specified range.</returns>
    public static IQueryable<T> PageRange<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        return (pageSize <= 0)
            ? query
            : query.Skip(pageNumber * pageSize).Take(pageSize);
    }
}
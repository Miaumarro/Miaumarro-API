using MiauAPI.Pagination;

namespace MiauAPI.Extensions;

/// <summary>
/// Provides extension methods for collections.
/// </summary>
public static class CollectionsExt
{
    /// <summary>
    /// Converts the current <paramref name="collection"/> to a <see cref="PagedList{T}"/>.
    /// </summary>
    /// <param name="collection">This collection.</param>
    /// <param name="pageNumber">The number of the desired page.</param>
    /// <param name="pageSize">The size of each page.</param>
    /// <typeparam name="T">The data type in the collection.</typeparam>
    /// <returns>A <see cref="PagedList{T}"/>.</returns>
    public static PagedList<T> ToPagedList<T>(this IEnumerable<T> collection, int pageNumber, int pageSize)
    {
        var count = collection.Count();
        var items = collection.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
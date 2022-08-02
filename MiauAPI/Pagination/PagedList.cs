namespace MiauAPI.Pagination;

/// <summary>
/// Define relevants parameters for the resulted query to be implemented as a PagedList.
/// </summary>
/// <param name="CurrentPage">The current page.</param>
/// <param name="TotalPages">The amount of pages result, given a <see cref="PageSize"/>.</param>
/// <param name="PageSize">The amount of results to be shown in a page.</param>
/// <param name="TotalCount">The amount of results.</param>
/// <param name="HasPrevious">Determines if the <see cref="CurrentPage"/> has a previou page.</param>
/// <param name="HasNext">Determines if the <see cref="CurrentPage"/> has a next page.</param>
public sealed class PagedList<T> : List<T>
{
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    /// <summary>
    /// The constructor of a PagedList.
    /// </summary>
    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        AddRange(items);
    }

    /// <summary>
    /// Converts the result query list to a PagedList
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public static PagedList<T> ToPagedList(List<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}

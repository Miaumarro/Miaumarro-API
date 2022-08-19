namespace MiauAPI.Pagination;

/// <summary>
/// Define relevants parameters for the resulted query to be implemented as a PagedList.
/// </summary>
public sealed class PagedList<T> : List<T>
{
    /// <summary>
    /// The current page.
    /// </summary>
    public int CurrentPage { get; }

    /// <summary>
    /// The amount of pages result, given a <see cref="PagedList{T}.PageSize"/>.
    /// </summary>
    public int TotalPages { get; }

    /// <summary>
    /// The amount of results to be shown in a page.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// The amount of results.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Determines if the <see cref="PagedList{T}.CurrentPage"/> has a previou page.
    /// </summary>
    public bool HasPrevious => CurrentPage > 1;

    /// <summary>
    /// Determines if the <see cref="PagedList{T}.CurrentPage"/> has a next page.
    /// </summary>
    public bool HasNext => CurrentPage < TotalPages - 1;

    /// <summary>
    /// The constructor of a PagedList.
    /// </summary>
    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        base.AddRange(items);
    }
}

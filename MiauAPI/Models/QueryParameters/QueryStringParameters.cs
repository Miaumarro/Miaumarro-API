namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Defines how the resulted query will be visualized.
/// </summary>
public abstract record QueryStringParameters
{
    // Default page size to 20
    private readonly int _pageSize = 20;

    /// <summary>
    /// The page to be visualized.
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// The amount of results to be shown in a page.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = Math.Min(value, 100);   // Cap page size to 100
    }
}
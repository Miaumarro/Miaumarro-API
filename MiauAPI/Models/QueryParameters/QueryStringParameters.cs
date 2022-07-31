namespace MiauAPI.Models.Parameters;

/// <summary>
/// Defines how the resulted query will be visualized.
/// </summary>
/// <param name="PageNumber">The page to be visualized.</param>
/// <param name="PageSize">The amount of results to be shown in a page.</param>
public abstract class QueryStringParameters
{
    private const int _maxPageSize = 50;
    private int _pageSize = 20;
    public int PageNumber { get; set; } = 1;
    public int PageSize 
    {
        get => _pageSize;
        set => _pageSize = (value > _maxPageSize) ? _maxPageSize : value;
    }
}

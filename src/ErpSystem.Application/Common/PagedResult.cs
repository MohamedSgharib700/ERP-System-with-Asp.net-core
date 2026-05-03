namespace ErpSystem.Application.Common;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
}

public class PagedQuery
{
    private int _pageSize = 20;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get => _pageSize; set => _pageSize = value > 100 ? 100 : value < 1 ? 20 : value; }
    public string? Search { get; set; }
}

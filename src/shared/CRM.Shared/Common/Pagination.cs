namespace CRM.Shared.Common;

/// <summary>
/// Pagination request/response primitives shared across all query handlers.
/// </summary>
public record PagedRequest(int Page = 1, int PageSize = 20);

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

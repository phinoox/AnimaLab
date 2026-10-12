// =============================================================================
namespace Anima.Core.Dtos.Shared;

/// <summary>
/// Generic list response wrapper used for paginated list endpoints.
/// </summary>
public class ListResponseDto<T>
{
/// <summary>
    /// The collection of items in the current page or full list.
/// </summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
/// <summary>
    /// Total number of records available across all pages.
/// </summary>
    public int TotalCount { get; set; }
}

// =============================================================================


/// <summary>
/// Generic paged response wrapper for list endpoints, including pagination metadata.
/// </summary>
public class PagedResponseDto<T> : ListResponseDto<T>
{
    /// <summary>
    /// The current page index (1-based).
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// The number of items requested per page.
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Total number of pages available based on the total count and page size.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

// =============================================================================


/// <summary>
/// Generic paginated list response wrapper for standardizing paged API responses.
/// </summary>
public class PaginatedListResponseDto<T> : PagedResponseDto<T>
{
    // No additional fields — just reuses PagedResponseDto behavior
}

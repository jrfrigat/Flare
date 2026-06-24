namespace Flare.Components;

/// <summary>Context for custom pagination info template.</summary>
public sealed class PaginationInfoContext
{
    /// <summary>Current page number (1-based).</summary>
    public int CurrentPage { get; init; }
    /// <summary>Items per page.</summary>
    public int PageSize { get; init; }
    /// <summary>Total number of items.</summary>
    public int TotalItems { get; init; }
    /// <summary>Total number of pages.</summary>
    public int TotalPages { get; init; }
    /// <summary>First item index on current page.</summary>
    public int From { get; init; }
    /// <summary>Last item index on current page.</summary>
    public int To { get; init; }
}

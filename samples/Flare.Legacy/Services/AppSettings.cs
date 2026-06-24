using Flare.Components;

namespace Flare.Legacy.Services;

/// <summary>How the data/form loading state is shown.</summary>
public enum LoadStyle
{
    /// <summary>Shimmering skeleton placeholders.</summary>
    Skeleton,
    /// <summary>A progress ring with a text label.</summary>
    Ring,
}

/// <summary>How the data grids page their rows.</summary>
public enum GridMode
{
    /// <summary>A classic pager at the bottom.</summary>
    Pagination,
    /// <summary>Rows are appended as the user scrolls to the bottom.</summary>
    InfiniteScroll,
}

/// <summary>App-wide demo settings (backend latency, loading style, grid mode, tab variant).
/// Singleton per session.</summary>
public sealed class AppSettings
{
    /// <summary>Simulated backend latency in milliseconds.</summary>
    public int BackendDelayMs { get; set; } = 800;

    /// <summary>How data/form loading is rendered.</summary>
    public LoadStyle LoadStyle { get; set; } = LoadStyle.Skeleton;

    /// <summary>How the grids page their rows (classic pager vs infinite scroll).</summary>
    public GridMode GridMode { get; set; } = GridMode.Pagination;

    /// <summary>Variant used for the document tabs.</summary>
    public TabsVariant TabVariant { get; set; } = TabsVariant.Underline;

    /// <summary>True when grids should append rows on scroll instead of paging.</summary>
    public bool InfiniteScroll => GridMode == GridMode.InfiniteScroll;

    /// <summary>The grid loading indicator that matches <see cref="LoadStyle"/>.</summary>
    public DataGridLoadingIndicator GridIndicator =>
        LoadStyle == LoadStyle.Skeleton ? DataGridLoadingIndicator.Skeleton : DataGridLoadingIndicator.Spinner;

    /// <summary>Raised when any setting changes; subscribers call StateHasChanged.</summary>
    public event Action? Changed;

    public void NotifyChanged() => Changed?.Invoke();
}

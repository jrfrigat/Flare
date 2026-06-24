namespace Flare.Components;

/// <summary>
/// Configuration for DataGrid virtualization and infinite scroll.
/// </summary>
public sealed class DataGridVirtualization
{
    /// <summary>Enable virtual scrolling for large datasets.</summary>
    public bool Enabled { get; set; }
    /// <summary>Enable infinite scroll (load more on scroll to bottom).</summary>
    public bool InfiniteScroll { get; set; }
    /// <summary>Height of the scroll container. Works with or without virtualization.</summary>
    public string Height { get; set; } = "400px";
    /// <summary>Minimum row count before virtualization kicks in.</summary>
    public int Threshold { get; set; } = 100;
    /// <summary>Estimated height of each row in pixels.</summary>
    public float ItemSize { get; set; } = 44f;
    /// <summary>Number of items to render above/below the visible area.</summary>
    public int OverscanCount { get; set; } = 6;
}

/// <summary>
/// Configuration for DataGrid loading states.
/// </summary>
public sealed class DataGridLoading
{
    /// <summary>Visual indicator shown while data is loading.</summary>
    public DataGridLoadingIndicator Indicator { get; set; } = DataGridLoadingIndicator.Spinner;
    /// <summary>Text displayed next to the loading indicator.</summary>
    public string? Text { get; set; }
    /// <summary>Number of skeleton rows shown during initial load.</summary>
    public int SkeletonRows { get; set; } = 8;
}

/// <summary>
/// Configuration for DataGrid appearance.
/// </summary>
public sealed class DataGridAppearance
{
    /// <summary>Alternating row background colors.</summary>
    public bool Striped { get; set; }
    /// <summary>Highlight row on hover.</summary>
    public bool Hoverable { get; set; }
    /// <summary>Compact row height and cell padding.</summary>
    public bool Dense { get; set; }
}

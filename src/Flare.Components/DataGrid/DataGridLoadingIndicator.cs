namespace Flare.Components;

/// <summary>How a <c>FlareDataGrid</c> shows its loading state.</summary>
public enum DataGridLoadingIndicator
{
    /// <summary>A centered circular progress ring overlaid on the (dimmed) table.</summary>
    Spinner,
    /// <summary>Shimmering skeleton placeholder rows in place of the data.</summary>
    Skeleton,
    /// <summary>A small text label only (no animation).</summary>
    Text,
    /// <summary>A thin indeterminate progress line at the top edge of the table; rows stay visible and interactive.</summary>
    ProgressLine,
}

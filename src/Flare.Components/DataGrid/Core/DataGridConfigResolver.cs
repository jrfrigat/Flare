namespace Flare.Components;

/// <summary>
/// Resolves the effective value from either a flat parameter or a grouped configuration.
/// Used internally by FlareDataGrid to support both old and new API styles.
/// </summary>
public static class DataGridConfigResolver
{
    /// <summary>Resolves whether row virtualization is effectively enabled.</summary>
    public static bool ResolveVirtual(bool? flat, DataGridVirtualization? grouped) =>
        grouped?.Enabled ?? flat ?? false;

    /// <summary>Resolves whether infinite scrolling is effectively enabled.</summary>
    public static bool ResolveInfiniteScroll(bool? flat, DataGridVirtualization? grouped) =>
        grouped?.InfiniteScroll ?? flat ?? false;

    /// <summary>Resolves the effective grid height.</summary>
    public static string ResolveHeight(string? flat, DataGridVirtualization? grouped, string defaultVal = "400px") =>
        grouped?.Height ?? flat ?? defaultVal;

    /// <summary>Resolves the row count above which virtualization turns on.</summary>
    public static int ResolveVirtualThreshold(int? flat, DataGridVirtualization? grouped, int defaultVal = 100) =>
        grouped?.Threshold ?? flat ?? defaultVal;

    /// <summary>Resolves the fixed row height (px) used by the virtualizer.</summary>
    public static float ResolveVirtualItemSize(float? flat, DataGridVirtualization? grouped, float defaultVal = 44f) =>
        grouped?.ItemSize ?? flat ?? defaultVal;

    /// <summary>Resolves how many extra rows the virtualizer renders outside the viewport.</summary>
    public static int ResolveOverscanCount(int? flat, DataGridVirtualization? grouped, int defaultVal = 6) =>
        grouped?.OverscanCount ?? flat ?? defaultVal;

    /// <summary>Resolves the loading-indicator style to show while data loads.</summary>
    public static DataGridLoadingIndicator ResolveLoadingIndicator(DataGridLoadingIndicator? flat, DataGridLoading? grouped) =>
        grouped?.Indicator ?? flat ?? DataGridLoadingIndicator.Spinner;

    /// <summary>Resolves the text shown while data loads.</summary>
    public static string? ResolveLoadingText(string? flat, DataGridLoading? grouped) =>
        grouped?.Text ?? flat;

    /// <summary>Resolves how many skeleton rows to render while data loads.</summary>
    public static int ResolveSkeletonRows(int? flat, DataGridLoading? grouped, int defaultVal = 8) =>
        grouped?.SkeletonRows ?? flat ?? defaultVal;

    /// <summary>Resolves whether row striping is effectively enabled.</summary>
    public static bool ResolveStriped(bool? flat, DataGridAppearance? grouped) =>
        grouped?.Striped ?? flat ?? false;

    /// <summary>Resolves whether row hover highlighting is effectively enabled.</summary>
    public static bool ResolveHoverable(bool? flat, DataGridAppearance? grouped) =>
        grouped?.Hoverable ?? flat ?? false;

    /// <summary>Resolves whether the dense (compact) row layout is effectively enabled.</summary>
    public static bool ResolveDense(bool? flat, DataGridAppearance? grouped) =>
        grouped?.Dense ?? flat ?? false;
}

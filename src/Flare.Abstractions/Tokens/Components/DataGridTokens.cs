namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for DataGrid component.
/// These control the geometry, spacing, and appearance of data grids.
/// </summary>
public sealed record DataGridTokens
{
    /// <summary>Background color of the grid surface.</summary>
    public string SurfaceColor { get; init; } = "var(--flare-color-surface)";

    /// <summary>Background color of the header row.</summary>
    public string HeaderBg { get; init; } = "var(--flare-color-surface-container)";

    /// <summary>Color of the header text.</summary>
    public string HeaderColor { get; init; } = "var(--flare-color-on-surface)";

    /// <summary>Font family of the header text.</summary>
    public string HeaderFontFamily { get; init; } = "var(--flare-typescale-label-medium-font)";

    /// <summary>Font size of the header text.</summary>
    public string HeaderFontSize { get; init; } = "var(--flare-typescale-label-medium-size)";

    /// <summary>Font weight of the header text.</summary>
    public string HeaderFontWeight { get; init; } = "var(--flare-typescale-label-medium-weight, 500)";

    /// <summary>Height of the header row.</summary>
    public string HeaderHeight { get; init; } = "56px";

    /// <summary>Padding inside header cells.</summary>
    public string HeaderPadding { get; init; } = "0 16px";

    /// <summary>Height of each data row.</summary>
    public string RowHeight { get; init; } = "52px";

    /// <summary>Dense row height (compact mode).</summary>
    public string RowHeightDense { get; init; } = "40px";

    /// <summary>Padding inside data cells.</summary>
    public string CellPadding { get; init; } = "0 16px";

    /// <summary>Color of the data cell text.</summary>
    public string CellColor { get; init; } = "var(--flare-color-on-surface)";

    /// <summary>Font family of the data cell text.</summary>
    public string CellFontFamily { get; init; } = "var(--flare-typescale-body-medium-font)";

    /// <summary>Font size of the data cell text.</summary>
    public string CellFontSize { get; init; } = "var(--flare-typescale-body-medium-size)";

    /// <summary>Background color of the selected row.</summary>
    public string SelectedRowBg { get; init; } = "var(--flare-color-primary-container)";

    /// <summary>Color of the selected row text.</summary>
    public string SelectedRowColor { get; init; } = "var(--flare-color-on-primary-container)";

    /// <summary>Background color of the hovered row.</summary>
    public string HoverRowBg { get; init; } = "color-mix(in srgb, var(--flare-color-on-surface) var(--flare-state-hover-opacity, 8%), transparent)";

    /// <summary>Color of the sort icon.</summary>
    public string SortIconColor { get; init; } = "var(--flare-color-on-surface-variant)";

    /// <summary>Color of the active sort icon.</summary>
    public string SortIconActiveColor { get; init; } = "var(--flare-color-primary)";

    /// <summary>Color of the grid lines/borders.</summary>
    public string BorderColor { get; init; } = "var(--flare-color-outline-variant)";

    /// <summary>Width of the grid lines/borders.</summary>
    public string BorderWidth { get; init; } = "1px";

    /// <summary>Background color of the filter row.</summary>
    public string FilterRowBg { get; init; } = "var(--flare-color-surface-container-low)";

    /// <summary>Background color of the group header.</summary>
    public string GroupHeaderBg { get; init; } = "var(--flare-color-surface-container)";

    /// <summary>Color of the group header text.</summary>
    public string GroupHeaderColor { get; init; } = "var(--flare-color-on-surface)";

    /// <summary>Background color of the toolbar.</summary>
    public string ToolbarBg { get; init; } = "var(--flare-color-surface)";

    /// <summary>Height of the toolbar.</summary>
    public string ToolbarHeight { get; init; } = "56px";

    /// <summary>Padding inside the toolbar.</summary>
    public string ToolbarPadding { get; init; } = "0 16px";

    /// <summary>Background color of the empty state.</summary>
    public string EmptyStateBg { get; init; } = "transparent";

    /// <summary>Color of the empty state text.</summary>
    public string EmptyStateColor { get; init; } = "var(--flare-color-on-surface-variant)";

    /// <summary>Width of the column resize handle.</summary>
    public string ResizeHandleWidth { get; init; } = "4px";

    /// <summary>Color of the column resize handle.</summary>
    public string ResizeHandleColor { get; init; } = "var(--flare-color-outline)";

    /// <summary>Background color of the column picker dropdown.</summary>
    public string ColumnPickerBg { get; init; } = "var(--flare-color-surface-container)";

    /// <summary>Elevation (box-shadow) of the column picker dropdown.</summary>
    public string ColumnPickerElevation { get; init; } = "var(--flare-elevation-2)";
}

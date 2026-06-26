using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for DataGrid component.
/// These control the geometry, spacing, and appearance of data grids.
/// </summary>
public sealed record DataGridTokens
{
    /// <summary>Background color of the grid surface.</summary>
    [CssVar(DataGridField.SurfaceColor)] public string SurfaceColor { get; init; } = "var(--flare-color-surface)";

    /// <summary>Background color of the header row.</summary>
    [CssVar(DataGridField.HeaderBg)] public string HeaderBg { get; init; } = "var(--flare-color-surface-container)";

    /// <summary>Color of the header text.</summary>
    [CssVar(DataGridField.HeaderColor)] public string HeaderColor { get; init; } = "var(--flare-color-on-surface)";

    /// <summary>Font family of the header text.</summary>
    [CssVar(DataGridField.HeaderFontFamily)] public string HeaderFontFamily { get; init; } = "var(--flare-typescale-label-medium-font)";

    /// <summary>Font size of the header text.</summary>
    [CssVar(DataGridField.HeaderFontSize)] public string HeaderFontSize { get; init; } = "var(--flare-typescale-label-medium-size)";

    /// <summary>Font weight of the header text.</summary>
    [CssVar(DataGridField.HeaderFontWeight)] public string HeaderFontWeight { get; init; } = "var(--flare-typescale-label-medium-weight, 500)";

    /// <summary>Height of the header row.</summary>
    [CssVar(DataGridField.HeaderHeight)] public string HeaderHeight { get; init; } = "56px";

    /// <summary>Padding inside header cells.</summary>
    [CssVar(DataGridField.HeaderPadding)] public string HeaderPadding { get; init; } = "0 16px";

    /// <summary>Height of each data row.</summary>
    [CssVar(DataGridField.RowHeight)] public string RowHeight { get; init; } = "52px";

    /// <summary>Dense row height (compact mode).</summary>
    [CssVar(DataGridField.RowHeightDense)] public string RowHeightDense { get; init; } = "40px";

    /// <summary>Padding inside data cells.</summary>
    [CssVar(DataGridField.CellPadding)] public string CellPadding { get; init; } = "0 16px";

    /// <summary>Color of the data cell text.</summary>
    [CssVar(DataGridField.CellColor)] public string CellColor { get; init; } = "var(--flare-color-on-surface)";

    /// <summary>Font family of the data cell text.</summary>
    [CssVar(DataGridField.CellFontFamily)] public string CellFontFamily { get; init; } = "var(--flare-typescale-body-medium-font)";

    /// <summary>Font size of the data cell text.</summary>
    [CssVar(DataGridField.CellFontSize)] public string CellFontSize { get; init; } = "var(--flare-typescale-body-medium-size)";

    /// <summary>Background color of the selected row.</summary>
    [CssVar(DataGridField.SelectedRowBg)] public string SelectedRowBg { get; init; } = "var(--flare-color-primary-container)";

    /// <summary>Color of the selected row text.</summary>
    [CssVar(DataGridField.SelectedRowColor)] public string SelectedRowColor { get; init; } = "var(--flare-color-on-primary-container)";

    /// <summary>Background color of the hovered row.</summary>
    [CssVar(DataGridField.HoverRowBg)] public string HoverRowBg { get; init; } = "color-mix(in srgb, var(--flare-color-on-surface) var(--flare-state-hover-opacity, 8%), transparent)";

    /// <summary>Color of the sort icon.</summary>
    [CssVar(DataGridField.SortIconColor)] public string SortIconColor { get; init; } = "var(--flare-color-on-surface-variant)";

    /// <summary>Color of the active sort icon.</summary>
    [CssVar(DataGridField.SortIconActiveColor)] public string SortIconActiveColor { get; init; } = "var(--flare-color-primary)";

    /// <summary>Color of the grid lines/borders.</summary>
    [CssVar(DataGridField.BorderColor)] public string BorderColor { get; init; } = "var(--flare-color-outline-variant)";

    /// <summary>Width of the grid lines/borders.</summary>
    [CssVar(DataGridField.BorderWidth)] public string BorderWidth { get; init; } = "1px";

    /// <summary>Background color of the filter row.</summary>
    [CssVar(DataGridField.FilterRowBg)] public string FilterRowBg { get; init; } = "var(--flare-color-surface-container-low)";

    /// <summary>Background color of the group header.</summary>
    [CssVar(DataGridField.GroupHeaderBg)] public string GroupHeaderBg { get; init; } = "var(--flare-color-surface-container)";

    /// <summary>Color of the group header text.</summary>
    [CssVar(DataGridField.GroupHeaderColor)] public string GroupHeaderColor { get; init; } = "var(--flare-color-on-surface)";

    /// <summary>Background color of the toolbar.</summary>
    [CssVar(DataGridField.ToolbarBg)] public string ToolbarBg { get; init; } = "var(--flare-color-surface)";

    /// <summary>Height of the toolbar.</summary>
    [CssVar(DataGridField.ToolbarHeight)] public string ToolbarHeight { get; init; } = "56px";

    /// <summary>Padding inside the toolbar.</summary>
    [CssVar(DataGridField.ToolbarPadding)] public string ToolbarPadding { get; init; } = "0 16px";

    /// <summary>Background color of the empty state.</summary>
    [CssVar(DataGridField.EmptyStateBg)] public string EmptyStateBg { get; init; } = "transparent";

    /// <summary>Color of the empty state text.</summary>
    [CssVar(DataGridField.EmptyStateColor)] public string EmptyStateColor { get; init; } = "var(--flare-color-on-surface-variant)";

    /// <summary>Width of the column resize handle.</summary>
    [CssVar(DataGridField.ResizeHandleWidth)] public string ResizeHandleWidth { get; init; } = "4px";

    /// <summary>Color of the column resize handle.</summary>
    [CssVar(DataGridField.ResizeHandleColor)] public string ResizeHandleColor { get; init; } = "var(--flare-color-outline)";

    /// <summary>Background color of the column picker dropdown.</summary>
    [CssVar(DataGridField.ColumnPickerBg)] public string ColumnPickerBg { get; init; } = "var(--flare-color-surface-container)";

    /// <summary>Elevation (box-shadow) of the column picker dropdown.</summary>
    [CssVar(DataGridField.ColumnPickerElevation)] public string ColumnPickerElevation { get; init; } = "var(--flare-elevation-2)";
}

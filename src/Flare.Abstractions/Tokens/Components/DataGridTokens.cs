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
    [CssVar(DataGridField.SurfaceColor)] public required string SurfaceColor { get; init; }

    /// <summary>Background color of the header row.</summary>
    [CssVar(DataGridField.HeaderBg)] public required string HeaderBg { get; init; }

    /// <summary>Color of the header text.</summary>
    [CssVar(DataGridField.HeaderColor)] public required string HeaderColor { get; init; }

    /// <summary>Font family of the header text.</summary>
    [CssVar(DataGridField.HeaderFontFamily)] public required string HeaderFontFamily { get; init; }

    /// <summary>Font size of the header text.</summary>
    [CssVar(DataGridField.HeaderFontSize)] public required string HeaderFontSize { get; init; }

    /// <summary>Font weight of the header text.</summary>
    [CssVar(DataGridField.HeaderFontWeight)] public required string HeaderFontWeight { get; init; }

    /// <summary>Height of the header row.</summary>
    [CssVar(DataGridField.HeaderHeight)] public required string HeaderHeight { get; init; }

    /// <summary>Padding inside header cells.</summary>
    [CssVar(DataGridField.HeaderPadding)] public required string HeaderPadding { get; init; }

    /// <summary>Height of each data row.</summary>
    [CssVar(DataGridField.RowHeight)] public required string RowHeight { get; init; }

    /// <summary>Dense row height (compact mode).</summary>
    [CssVar(DataGridField.RowHeightDense)] public required string RowHeightDense { get; init; }

    /// <summary>Padding inside data cells.</summary>
    [CssVar(DataGridField.CellPadding)] public required string CellPadding { get; init; }

    /// <summary>Color of the data cell text.</summary>
    [CssVar(DataGridField.CellColor)] public required string CellColor { get; init; }

    /// <summary>Font family of the data cell text.</summary>
    [CssVar(DataGridField.CellFontFamily)] public required string CellFontFamily { get; init; }

    /// <summary>Font size of the data cell text.</summary>
    [CssVar(DataGridField.CellFontSize)] public required string CellFontSize { get; init; }

    /// <summary>Background color of the selected row.</summary>
    [CssVar(DataGridField.SelectedRowBg)] public required string SelectedRowBg { get; init; }

    /// <summary>Color of the selected row text.</summary>
    [CssVar(DataGridField.SelectedRowColor)] public required string SelectedRowColor { get; init; }

    /// <summary>Background color of the hovered row.</summary>
    [CssVar(DataGridField.HoverRowBg)] public required string HoverRowBg { get; init; }

    /// <summary>Color of the sort icon.</summary>
    [CssVar(DataGridField.SortIconColor)] public required string SortIconColor { get; init; }

    /// <summary>Color of the active sort icon.</summary>
    [CssVar(DataGridField.SortIconActiveColor)] public required string SortIconActiveColor { get; init; }

    /// <summary>Color of the grid lines/borders.</summary>
    [CssVar(DataGridField.BorderColor)] public required string BorderColor { get; init; }

    /// <summary>Width of the grid lines/borders.</summary>
    [CssVar(DataGridField.BorderWidth)] public required string BorderWidth { get; init; }

    /// <summary>Background color of the filter row.</summary>
    [CssVar(DataGridField.FilterRowBg)] public required string FilterRowBg { get; init; }

    /// <summary>Background color of the group header.</summary>
    [CssVar(DataGridField.GroupHeaderBg)] public required string GroupHeaderBg { get; init; }

    /// <summary>Color of the group header text.</summary>
    [CssVar(DataGridField.GroupHeaderColor)] public required string GroupHeaderColor { get; init; }

    /// <summary>Background color of the toolbar.</summary>
    [CssVar(DataGridField.ToolbarBg)] public required string ToolbarBg { get; init; }

    /// <summary>Height of the toolbar.</summary>
    [CssVar(DataGridField.ToolbarHeight)] public required string ToolbarHeight { get; init; }

    /// <summary>Padding inside the toolbar.</summary>
    [CssVar(DataGridField.ToolbarPadding)] public required string ToolbarPadding { get; init; }

    /// <summary>Background color of the empty state.</summary>
    [CssVar(DataGridField.EmptyStateBg)] public required string EmptyStateBg { get; init; }

    /// <summary>Color of the empty state text.</summary>
    [CssVar(DataGridField.EmptyStateColor)] public required string EmptyStateColor { get; init; }

    /// <summary>Width of the column resize handle.</summary>
    [CssVar(DataGridField.ResizeHandleWidth)] public required string ResizeHandleWidth { get; init; }

    /// <summary>Color of the column resize handle.</summary>
    [CssVar(DataGridField.ResizeHandleColor)] public required string ResizeHandleColor { get; init; }

    /// <summary>Background color of the column picker dropdown.</summary>
    [CssVar(DataGridField.ColumnPickerBg)] public required string ColumnPickerBg { get; init; }

    /// <summary>Elevation (box-shadow) of the column picker dropdown.</summary>
    [CssVar(DataGridField.ColumnPickerElevation)] public required string ColumnPickerElevation { get; init; }
}

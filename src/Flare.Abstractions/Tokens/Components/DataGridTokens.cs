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
    [CssVar(DataGridField.SurfaceColor)] public string SurfaceColor { get; init; } = Vars.Var(Color.Surface);

    /// <summary>Background color of the header row.</summary>
    [CssVar(DataGridField.HeaderBg)] public string HeaderBg { get; init; } = Vars.Var(Color.SurfaceContainer);

    /// <summary>Color of the header text.</summary>
    [CssVar(DataGridField.HeaderColor)] public string HeaderColor { get; init; } = Vars.Var(Color.OnSurface);

    /// <summary>Font family of the header text.</summary>
    [CssVar(DataGridField.HeaderFontFamily)] public string HeaderFontFamily { get; init; } = Vars.Var(Typography.Font("label-medium"));

    /// <summary>Font size of the header text.</summary>
    [CssVar(DataGridField.HeaderFontSize)] public string HeaderFontSize { get; init; } = Vars.Var(Typography.Size("label-medium"));

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
    [CssVar(DataGridField.CellColor)] public string CellColor { get; init; } = Vars.Var(Color.OnSurface);

    /// <summary>Font family of the data cell text.</summary>
    [CssVar(DataGridField.CellFontFamily)] public string CellFontFamily { get; init; } = Vars.Var(Typography.Font("body-medium"));

    /// <summary>Font size of the data cell text.</summary>
    [CssVar(DataGridField.CellFontSize)] public string CellFontSize { get; init; } = Vars.Var(Typography.Size("body-medium"));

    /// <summary>Background color of the selected row.</summary>
    [CssVar(DataGridField.SelectedRowBg)] public string SelectedRowBg { get; init; } = Vars.Var(Color.PrimaryContainer);

    /// <summary>Color of the selected row text.</summary>
    [CssVar(DataGridField.SelectedRowColor)] public string SelectedRowColor { get; init; } = Vars.Var(Color.OnPrimaryContainer);

    /// <summary>Background color of the hovered row.</summary>
    [CssVar(DataGridField.HoverRowBg)] public string HoverRowBg { get; init; } = "color-mix(in srgb, var(--flare-color-on-surface) var(--flare-state-hover-opacity, 8%), transparent)";

    /// <summary>Color of the sort icon.</summary>
    [CssVar(DataGridField.SortIconColor)] public string SortIconColor { get; init; } = Vars.Var(Color.OnSurfaceVariant);

    /// <summary>Color of the active sort icon.</summary>
    [CssVar(DataGridField.SortIconActiveColor)] public string SortIconActiveColor { get; init; } = Vars.Var(Color.Primary);

    /// <summary>Color of the grid lines/borders.</summary>
    [CssVar(DataGridField.BorderColor)] public string BorderColor { get; init; } = Vars.Var(Color.OutlineVariant);

    /// <summary>Width of the grid lines/borders.</summary>
    [CssVar(DataGridField.BorderWidth)] public string BorderWidth { get; init; } = "1px";

    /// <summary>Background color of the filter row.</summary>
    [CssVar(DataGridField.FilterRowBg)] public string FilterRowBg { get; init; } = Vars.Var(Color.SurfaceContainerLow);

    /// <summary>Background color of the group header.</summary>
    [CssVar(DataGridField.GroupHeaderBg)] public string GroupHeaderBg { get; init; } = Vars.Var(Color.SurfaceContainer);

    /// <summary>Color of the group header text.</summary>
    [CssVar(DataGridField.GroupHeaderColor)] public string GroupHeaderColor { get; init; } = Vars.Var(Color.OnSurface);

    /// <summary>Background color of the toolbar.</summary>
    [CssVar(DataGridField.ToolbarBg)] public string ToolbarBg { get; init; } = Vars.Var(Color.Surface);

    /// <summary>Height of the toolbar.</summary>
    [CssVar(DataGridField.ToolbarHeight)] public string ToolbarHeight { get; init; } = "56px";

    /// <summary>Padding inside the toolbar.</summary>
    [CssVar(DataGridField.ToolbarPadding)] public string ToolbarPadding { get; init; } = "0 16px";

    /// <summary>Background color of the empty state.</summary>
    [CssVar(DataGridField.EmptyStateBg)] public string EmptyStateBg { get; init; } = "transparent";

    /// <summary>Color of the empty state text.</summary>
    [CssVar(DataGridField.EmptyStateColor)] public string EmptyStateColor { get; init; } = Vars.Var(Color.OnSurfaceVariant);

    /// <summary>Width of the column resize handle.</summary>
    [CssVar(DataGridField.ResizeHandleWidth)] public string ResizeHandleWidth { get; init; } = "4px";

    /// <summary>Color of the column resize handle.</summary>
    [CssVar(DataGridField.ResizeHandleColor)] public string ResizeHandleColor { get; init; } = Vars.Var(Color.Outline);

    /// <summary>Background color of the column picker dropdown.</summary>
    [CssVar(DataGridField.ColumnPickerBg)] public string ColumnPickerBg { get; init; } = Vars.Var(Color.SurfaceContainer);

    /// <summary>Elevation (box-shadow) of the column picker dropdown.</summary>
    [CssVar(DataGridField.ColumnPickerElevation)] public string ColumnPickerElevation { get; init; } = Vars.Var(Elevation.Level2);
}

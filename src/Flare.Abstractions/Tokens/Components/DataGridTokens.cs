using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the DataGrid. Color, font, backgrounds, padding and row heights are intentionally NOT
/// tokens here - the grid reuses the shared color roles, typescale, spacing and state scales directly. Only
/// grid-specific geometry (icon sizes, divider/handle widths, overlay percentages) are tokens.
/// </summary>
public sealed record DataGridTokens
{
    /// <summary>Size of the column sort-direction icon.</summary>
    [CssVar(DataGridField.SortIconSize)] public required string SortIconSize { get; init; }

    /// <summary>Size of the multi-sort priority badge.</summary>
    [CssVar(DataGridField.SortPrioritySize)] public required string SortPrioritySize { get; init; }

    /// <summary>Size of the filter icon.</summary>
    [CssVar(DataGridField.FilterIconSize)] public required string FilterIconSize { get; init; }

    /// <summary>Size of the boolean-cell check/cross icon.</summary>
    [CssVar(DataGridField.BoolIconSize)] public required string BoolIconSize { get; init; }

    /// <summary>Size of toolbar-button icons.</summary>
    [CssVar(DataGridField.BtnIconSize)] public required string BtnIconSize { get; init; }

    /// <summary>Size of the close icon.</summary>
    [CssVar(DataGridField.CloseIconSize)] public required string CloseIconSize { get; init; }

    /// <summary>Size of the group/tree expand chevron.</summary>
    [CssVar(DataGridField.ChevronSize)] public required string ChevronSize { get; init; }

    /// <summary>Size of the detail-row toggle icon.</summary>
    [CssVar(DataGridField.DetailIconSize)] public required string DetailIconSize { get; init; }

    /// <summary>Size of the tree-toggle icon.</summary>
    [CssVar(DataGridField.TreeToggleSize)] public required string TreeToggleSize { get; init; }

    /// <summary>Font size of a composite-column secondary label.</summary>
    [CssVar(DataGridField.CompositeLabelSize)] public required string CompositeLabelSize { get; init; }

    /// <summary>Width of the column resize handle.</summary>
    [CssVar(DataGridField.ResizeHandleWidth)] public required string ResizeHandleWidth { get; init; }

    /// <summary>Width of the divider between records in card mode.</summary>
    [CssVar(DataGridField.RecordDividerWidth)] public required string RecordDividerWidth { get; init; }

    /// <summary>Width of the aggregate-row divider.</summary>
    [CssVar(DataGridField.AggregateDividerWidth)] public required string AggregateDividerWidth { get; init; }

    /// <summary>Width of the filter-group accent rail.</summary>
    [CssVar(DataGridField.FilterGroupRail)] public required string FilterGroupRail { get; init; }

    /// <summary>Focus outline of the active (keyboard) cell.</summary>
    [CssVar(DataGridField.ActiveCellOutline)] public required string ActiveCellOutline { get; init; }

    /// <summary>Minimum width of the column-picker dropdown.</summary>
    [CssVar(DataGridField.ColumnPickerMinWidth)] public required string ColumnPickerMinWidth { get; init; }

    /// <summary>Mix percentage for a row that is both selected and hovered.</summary>
    [CssVar(DataGridField.RowSelectedHoverPct)] public required string RowSelectedHoverPct { get; init; }

    /// <summary>Tint percentage for the row currently being edited.</summary>
    [CssVar(DataGridField.RowEditingPct)] public required string RowEditingPct { get; init; }

    /// <summary>Opacity percentage of the loading veil.</summary>
    [CssVar(DataGridField.LoadingVeilPct)] public required string LoadingVeilPct { get; init; }

    /// <summary>Opacity of the dimmed content behind the loading veil.</summary>
    [CssVar(DataGridField.LoadingDim)] public required string LoadingDim { get; init; }
}

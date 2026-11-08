namespace Flare.Components;

/// <summary>How <c>FlareDataGrid</c> presents its records at the current width.</summary>
public enum DataGridPresentation
{
    /// <summary>The ordinary table. This is the default and preserves the existing behavior.</summary>
    Table,
    /// <summary>A compact presentation: each row becomes a record, each cell carries its column title,
    /// and the table header becomes a compact control bar. Sorting, filtering, selection and
    /// virtualization keep working.</summary>
    Stacked,
    /// <summary>Uses <see cref="Stacked"/> while the grid's own width is below
    /// <c>FlareDataGrid.PresentationBreakpoint</c>, and <see cref="Table"/> at or above it.</summary>
    Auto,
}

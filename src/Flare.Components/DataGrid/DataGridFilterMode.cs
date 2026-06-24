namespace Flare.Components;

/// <summary>
/// Controls how column filters are displayed in the DataGrid.
/// </summary>
public enum DataGridFilterMode
{
    /// <summary>Show filter row with text inputs below headers (default).</summary>
    Simple,
    /// <summary>Show filter icon in column headers that opens a dropdown with filter options.</summary>
    Menu,
}

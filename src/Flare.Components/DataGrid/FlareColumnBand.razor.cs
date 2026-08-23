namespace Flare.Components;

/// <summary>
/// A heading that spans several columns, turning a flat header into a banded one. It draws no cells of
/// its own: nested columns and bands register into it, and the grid reads that tree to lay the header
/// rows out. Bands may nest to any depth.
/// </summary>
public partial class FlareColumnBand;

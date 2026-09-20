namespace Flare.Components;

/// <summary>
/// Emits the stylesheet links a theme needs in the document head. By default only the active theme and
/// palette, because that is all the first frame paints with and every other sheet is a request nothing
/// uses; switching theme still works, since the provider fetches the incoming theme's sheet before it
/// swaps the classes.
/// </summary>
public partial class FlareStyles;

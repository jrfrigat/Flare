namespace Flare.Components;

/// <summary>
/// The browser viewport size in CSS pixels (<c>window.innerWidth</c> / <c>window.innerHeight</c>).
/// </summary>
/// <param name="Width">Viewport width in pixels.</param>
/// <param name="Height">Viewport height in pixels.</param>
public readonly record struct ViewportSize(int Width, int Height)
{
    /// <summary>True when the viewport is taller than it is wide.</summary>
    public bool IsPortrait => Height > Width;

    /// <summary>True when the viewport is wider than it is tall.</summary>
    public bool IsLandscape => Width >= Height;

    /// <summary>The tier this width resolves to on the default breakpoint scale.</summary>
    public Breakpoint Breakpoint => FlareBreakpoints.FromWidth(Width);
}

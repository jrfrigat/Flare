namespace Flare.Components;

/// <summary>
/// Source-compatible alias for <see cref="FlareSize"/>: <c>ButtonSize.Xs</c> resolves to the same
/// value, so existing markup keeps compiling. New code uses <see cref="FlareSize"/> directly; the
/// button size semantics live there.
/// </summary>
public static class ButtonSize
{
    /// <summary>Extra small - <see cref="FlareSize.Xs"/>.</summary>
    public const FlareSize Xs = FlareSize.Xs;
    /// <summary>Small - <see cref="FlareSize.Sm"/>.</summary>
    public const FlareSize Sm = FlareSize.Sm;
    /// <summary>Medium - <see cref="FlareSize.Md"/>.</summary>
    public const FlareSize Md = FlareSize.Md;
    /// <summary>Large - <see cref="FlareSize.Lg"/>.</summary>
    public const FlareSize Lg = FlareSize.Lg;
    /// <summary>Extra large - <see cref="FlareSize.Xl"/>.</summary>
    public const FlareSize Xl = FlareSize.Xl;
}

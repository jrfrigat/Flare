namespace Flare.Icons;

/// <summary>
/// Built-in <see cref="FlareMorphIcon"/> pairs - shapes drawn against each other so their outlines
/// interpolate instead of swapping. Each pair below shares one command list, which is the whole
/// requirement; the members are meant to be used together, e.g.
/// <c>&lt;FlareIconView Value="@(open ? FlareMorphIcons.Minus : FlareMorphIcons.Plus)" /&gt;</c>.
/// </summary>
/// <remarks>
/// The shapes are drawn with absolute line segments only (no arcs, no curves, no shorthand), because
/// that is the form every engine interpolates without argument. The simpler shape of each pair carries
/// the same number of points as the richer one, with the surplus points collapsed onto each other - a
/// zero-length segment is invisible, and it is what lets a bar have as many corners as a cross.
/// </remarks>
public static class FlareMorphIcons
{
    /// <summary>A plus sign that collapses into <see cref="Minus"/>.</summary>
    public static FlareMorphIcon Plus { get; } = new()
    {
        Data = "M11 5L13 5L13 11L19 11L19 13L13 13L13 19L11 19L11 13L5 13L5 11L11 11Z",
    };

    /// <summary>A minus sign that grows into <see cref="Plus"/>; the vertical arm is folded into the bar.</summary>
    public static FlareMorphIcon Minus { get; } = new()
    {
        Data = "M11 11L13 11L13 11L19 11L19 13L13 13L13 13L11 13L11 13L5 13L5 11L11 11Z",
    };

    /// <summary>A chevron pointing down, the counterpart of <see cref="ChevronUp"/>.</summary>
    public static FlareMorphIcon ChevronDown { get; } = new()
    {
        Data = "M6 9.4L7.4 8L12 12.6L16.6 8L18 9.4L12 15.4Z",
    };

    /// <summary>A chevron pointing up; the same outline as <see cref="ChevronDown"/> reflected, so the pair turns over rather than swapping.</summary>
    public static FlareMorphIcon ChevronUp { get; } = new()
    {
        Data = "M6 14.6L7.4 16L12 11.4L16.6 16L18 14.6L12 8.6Z",
    };
}

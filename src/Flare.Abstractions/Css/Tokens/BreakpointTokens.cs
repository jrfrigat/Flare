namespace Flare.Css.Tokens;

/// <summary>
/// Read-only CSS custom properties exposing each responsive breakpoint's lower bound (min-width, px).
/// The values mirror the media-query boundaries in <c>responsive.css</c> / <c>grid.css</c> and the C#
/// <see cref="Flare.Components.FlareBreakpoints"/> scale, so CSS and code agree on the same thresholds.
/// </summary>
public static class Breakpoint
{
    /// <summary>Smallest tier lower bound (<c>0</c>).</summary>
    public const string Xs = "--flare-breakpoint-xs";
    /// <summary>Small tier lower bound (<c>600px</c>).</summary>
    public const string Sm = "--flare-breakpoint-sm";
    /// <summary>Medium tier lower bound (<c>960px</c>).</summary>
    public const string Md = "--flare-breakpoint-md";
    /// <summary>Large tier lower bound (<c>1280px</c>).</summary>
    public const string Lg = "--flare-breakpoint-lg";
    /// <summary>Extra-large tier lower bound (<c>1920px</c>).</summary>
    public const string Xl = "--flare-breakpoint-xl";
    /// <summary>Extra-extra-large tier lower bound (<c>2560px</c>).</summary>
    public const string Xxl = "--flare-breakpoint-xxl";
}

namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for breakpoint.</summary>
public static class Breakpoint
{
    private const string Prefix = $"{Vars.Flare}-breakpoint";

    /// <summary>Mobile breakpoint (max-width: 480px).</summary>
    public const string Xs = $"{Prefix}-xs";
    /// <summary>Tablet breakpoint (max-width: 768px).</summary>
    public const string Sm = $"{Prefix}-sm";
    /// <summary>Desktop breakpoint (max-width: 1024px).</summary>
    public const string Md = $"{Prefix}-md";
    /// <summary>Large desktop breakpoint (max-width: 1280px).</summary>
    public const string Lg = $"{Prefix}-lg";
    /// <summary>Extra large breakpoint (min-width: 1281px).</summary>
    public const string Xl = $"{Prefix}-xl";
}

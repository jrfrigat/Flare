namespace Flare.Css.Tokens;

/// <summary>Root CSS custom-property names and the <c>var(...)</c> helper for the token system.</summary>
public static class Vars
{
    /// <summary>CSS custom-property name prefix <c>--flare</c> (the base every token name builds on).</summary>
    public const string Flare = "--flare";

    /// <summary>CSS custom-property name <c>--flare-focus-ring</c>.</summary>
    public const string FocusRing = "--flare-focus-ring";

    /// <summary>Wraps the given CSS custom-property name in a <c>var(...)</c> expression.</summary>
    public static string Var(string token) => $"var({token})";
}

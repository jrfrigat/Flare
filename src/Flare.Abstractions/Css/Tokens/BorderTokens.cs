namespace Flare.Css.Tokens;

/// <summary>
/// CSS variable tokens for the border scale - the rules Flare draws between and around things.
/// <para>
/// Three primitives and two ready-made shorthands, not a token per component. Before these existed the
/// stylesheets carried 86 literal <c>1px solid</c> / <c>2px solid</c> declarations, so a language whose
/// separators are hairlines, or double rules, or absent altogether, had no way to say so; the colour was
/// a token and everything else was core's opinion. A component whose rule has its own colour token still
/// composes <see cref="Width"/> and <see cref="Style"/> with that colour rather than restating a length.
/// </para>
/// </summary>
public static class Border
{
    /// <summary>CSS custom-property name for the standard rule width.</summary>
    public const string Width = "--flare-border-width";
    /// <summary>CSS custom-property name for the emphasised rule width (selection, drop targets, the
    /// active edge of a stepper).</summary>
    public const string WidthEmphasis = "--flare-border-width-emphasis";
    /// <summary>CSS custom-property name for the rule's line style.</summary>
    public const string Style = "--flare-border-style";
    /// <summary>CSS custom-property name for the divider rule, as a <c>border</c> shorthand: the hairline
    /// between rows, cells, sections and panes.</summary>
    public const string Divider = "--flare-border-divider";
    /// <summary>CSS custom-property name for the container rule, as a <c>border</c> shorthand: the edge a
    /// surface draws around itself, stronger than a divider.</summary>
    public const string Outline = "--flare-border-outline";
}

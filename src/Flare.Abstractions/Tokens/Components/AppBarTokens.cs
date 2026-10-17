using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for appbar - component-specific geometry read by appbar.css.</summary>
public sealed record AppBarTokens
{
    /// <summary>Gap.</summary>
    [CssVar(AppBarField.Gap)] public string Gap { get; init; } = "var(--flare-spacing-2)";

    /// <summary>Height.</summary>
    [CssVar(AppBarField.Height)] public required string Height { get; init; }

    /// <summary>Height Dense.</summary>
    [CssVar(AppBarField.HeightDense)] public required string HeightDense { get; init; }

    /// <summary>Padding X.</summary>
    [CssVar(AppBarField.PaddingX)] public string PaddingX { get; init; } = "var(--flare-spacing-2)";

    /// <summary>Title Padding X.</summary>
    [CssVar(AppBarField.TitlePaddingX)] public string TitlePaddingX { get; init; } = "var(--flare-spacing-6)";

    /// <summary>Bottom edge of the bar, as a <c>border</c> shorthand. <c>none</c> for a language that
    /// separates the bar from the content by elevation or surface tone rather than by a rule.</summary>
    [CssVar(AppBarField.Border)] public required string Border { get; init; }
}

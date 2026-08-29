using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for appbar - component-specific geometry read by appbar.css.</summary>
public sealed record AppBarTokens
{
    /// <summary>Gap.</summary>
    [CssVar(AppBarField.Gap)] public required string Gap { get; init; }

    /// <summary>Height.</summary>
    [CssVar(AppBarField.Height)] public required string Height { get; init; }

    /// <summary>Height Dense.</summary>
    [CssVar(AppBarField.HeightDense)] public required string HeightDense { get; init; }

    /// <summary>Padding X.</summary>
    [CssVar(AppBarField.PaddingX)] public required string PaddingX { get; init; }

    /// <summary>Title Padding X.</summary>
    [CssVar(AppBarField.TitlePaddingX)] public required string TitlePaddingX { get; init; }

    /// <summary>Bottom edge of the bar, as a <c>border</c> shorthand. <c>none</c> for a language that
    /// separates the bar from the content by elevation or surface tone rather than by a rule.</summary>
    [CssVar(AppBarField.Border)] public required string Border { get; init; }
}

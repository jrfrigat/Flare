using Flare.Css;
using Flare.Css.Tokens;
using static Flare.Css.Tokens.Vars;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareCarousel</c> - the viewport, its navigation arrows and dot indicators.</summary>
public sealed record CarouselTokens
{
    /// <summary>Corner radius of the carousel container.</summary>
    [CssVar(CarouselField.Radius)] public string Radius { get; init; } = Var(Css.Tokens.Shape.Medium);
    /// <summary>Background of the carousel container.</summary>
    [CssVar(CarouselField.Background)] public string Background { get; init; } = Var(Css.Tokens.Color.SurfaceContainerLow);

    /// <summary>Width and height of a navigation arrow button.</summary>
    [CssVar(CarouselField.ArrowSize)] public string ArrowSize { get; init; } = Var(Css.Tokens.Spacing.S20);
    /// <summary>Distance between a navigation arrow and the carousel's inline edge.</summary>
    [CssVar(CarouselField.ArrowInset)] public string ArrowInset { get; init; } = Var(Css.Tokens.Spacing.S4);
    /// <summary>Background of a navigation arrow over the slide content.</summary>
    [CssVar(CarouselField.ArrowBackground)] public string ArrowBackground { get; init; } =
        $"color-mix(in srgb, {Var(Css.Tokens.Color.Surface)} 85%, transparent)";
    /// <summary>Background of a navigation arrow while it is hovered.</summary>
    [CssVar(CarouselField.ArrowBackgroundHover)] public string ArrowBackgroundHover { get; init; } =
        $"color-mix(in srgb, {Var(Css.Tokens.Color.OnSurface)} 12%, {Var(Css.Tokens.Color.Surface)})";
    /// <summary>Color of the arrow glyph.</summary>
    [CssVar(CarouselField.ArrowColor)] public string ArrowColor { get; init; } = Var(Css.Tokens.Color.OnSurface);
    /// <summary>Border around a navigation arrow.</summary>
    [CssVar(CarouselField.ArrowBorder)] public string ArrowBorder { get; init; } =
        $"{Var(Css.Tokens.Border.Width)} {Var(Css.Tokens.Border.Style)} {Var(Css.Tokens.Color.OutlineVariant)}";
    /// <summary>Opacity of a disabled navigation arrow.</summary>
    [CssVar(CarouselField.ArrowDisabledOpacity)] public string ArrowDisabledOpacity { get; init; } = Var(Css.Tokens.State.DisabledOpacity);
    /// <summary>Backdrop blur applied to a navigation arrow; no base length scale carries it, so a theme must set it.</summary>
    [CssVar(CarouselField.ArrowBlur)] public required string ArrowBlur { get; init; }
    /// <summary>Corner radius of a navigation arrow.</summary>
    [CssVar(CarouselField.ArrowRadius)] public string ArrowRadius { get; init; } = Var(Css.Tokens.Shape.Full);

    /// <summary>Distance between the dot row and the carousel's bottom edge.</summary>
    [CssVar(CarouselField.DotsBottom)] public string DotsBottom { get; init; } = Var(Css.Tokens.Spacing.S6);
    /// <summary>Space between adjacent dots.</summary>
    [CssVar(CarouselField.DotsGap)] public string DotsGap { get; init; } = Var(Css.Tokens.Spacing.S3);
    /// <summary>Width and height of an inactive dot.</summary>
    [CssVar(CarouselField.DotSize)] public string DotSize { get; init; } = Var(Css.Tokens.Spacing.S4);
    /// <summary>Width of the active dot.</summary>
    [CssVar(CarouselField.DotActiveWidth)] public string DotActiveWidth { get; init; } = Var(Css.Tokens.Spacing.S10);
    /// <summary>Background of an inactive dot.</summary>
    [CssVar(CarouselField.DotBackground)] public string DotBackground { get; init; } =
        $"color-mix(in srgb, {Var(Css.Tokens.Color.OnSurface)} 40%, transparent)";
    /// <summary>Background of the active dot.</summary>
    [CssVar(CarouselField.DotActiveBackground)] public string DotActiveBackground { get; init; } = Var(Css.Tokens.Color.Primary);

    /// <summary>Focus ring drawn around a carousel control.</summary>
    [CssVar(CarouselField.FocusRing)] public string FocusRing { get; init; } = Var(Css.Tokens.Vars.FocusRing);
    /// <summary>Distance between a focused control and its focus ring.</summary>
    [CssVar(CarouselField.FocusRingOffset)] public string FocusRingOffset { get; init; } = Var(Css.Tokens.Spacing.S1);
}

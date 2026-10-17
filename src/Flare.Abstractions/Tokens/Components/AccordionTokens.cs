using Flare.Css;
using Flare.Css.Tokens;
using static Flare.Css.Tokens.Vars;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareAccordion</c> / <c>FlareAccordionPanel</c> (the container, its
/// panel headers and their bodies).</summary>
public sealed record AccordionTokens
{
    /// <summary>Border around the accordion container, as a CSS <c>border</c> shorthand. A theme with a
    /// borderless stack of sections parks this at <c>none</c>.</summary>
    [CssVar(AccordionField.Border)] public string Border { get; init; } = $"{Var(Css.Tokens.Border.Width)} {Var(Css.Tokens.Border.Style)} {Var(Css.Tokens.Color.OutlineVariant)}";
    /// <summary>Corner radius of the accordion container, its own rather than the card's, so a theme can
    /// reshape one without reshaping the other.</summary>
    [CssVar(AccordionField.Radius)] public string Radius { get; init; } = Var(Css.Tokens.Shape.Medium);
    /// <summary>Rule drawn between adjacent panels, as a CSS <c>border</c> shorthand.</summary>
    [CssVar(AccordionField.PanelDivider)] public string PanelDivider { get; init; } = $"{Var(Css.Tokens.Border.Width)} {Var(Css.Tokens.Border.Style)} {Var(Css.Tokens.Color.OutlineVariant)}";

    /// <summary>Background of a panel header.</summary>
    [CssVar(AccordionField.HeaderBg)] public string HeaderBg { get; init; } = Var(Css.Tokens.Color.Surface);
    /// <summary>Foreground of a panel header.</summary>
    [CssVar(AccordionField.HeaderColor)] public string HeaderColor { get; init; } = Var(Css.Tokens.Color.OnSurface);
    /// <summary>Space above and below a panel header's content.</summary>
    [CssVar(AccordionField.HeaderPaddingBlock)] public string HeaderPaddingBlock { get; init; } = Var(Css.Tokens.Spacing.S8);
    /// <summary>Space between a panel header's side edges and its content.</summary>
    [CssVar(AccordionField.HeaderPaddingInline)] public string HeaderPaddingInline { get; init; } = Var(Css.Tokens.Spacing.S12);
    /// <summary>Space between the parts of a header's inline content (icons, badges, the title).</summary>
    [CssVar(AccordionField.HeaderGap)] public string HeaderGap { get; init; } = Var(Css.Tokens.Spacing.S4);
    /// <summary>Font family of a panel header. Each theme decides which step of its own type scale a
    /// section title maps to.</summary>
    [CssVar(AccordionField.HeaderLabelFont)] public string HeaderLabelFont { get; init; } = Var(Css.Tokens.Typography.TitleSmall.Font);
    /// <summary>Font size of a panel header.</summary>
    [CssVar(AccordionField.HeaderLabelSize)] public string HeaderLabelSize { get; init; } = Var(Css.Tokens.Typography.TitleSmall.Size);
    /// <summary>Font weight of a panel header.</summary>
    [CssVar(AccordionField.HeaderLabelWeight)] public string HeaderLabelWeight { get; init; } = Var(Css.Tokens.Typography.TitleSmall.Weight);
    /// <summary>How far a disabled panel header fades. A language that repaints disabled headers in a
    /// flat palette leaves this opaque and carries the change in its own stylesheet, since a foreground
    /// colour has no value meaning "leave this as painted".</summary>
    [CssVar(AccordionField.HeaderDisabledOpacity)] public string HeaderDisabledOpacity { get; init; } = Var(Css.Tokens.State.DisabledOpacity);
    /// <summary>Size of the chevron that turns as a panel expands.</summary>
    [CssVar(AccordionField.IconSize)] public required string IconSize { get; init; }

    /// <summary>Space above and below a panel body's content.</summary>
    [CssVar(AccordionField.BodyPaddingBlock)] public string BodyPaddingBlock { get; init; } = Var(Css.Tokens.Spacing.S8);
    /// <summary>Space between a panel body's side edges and its content.</summary>
    [CssVar(AccordionField.BodyPaddingInline)] public string BodyPaddingInline { get; init; } = Var(Css.Tokens.Spacing.S12);
    /// <summary>Foreground of a panel body.</summary>
    [CssVar(AccordionField.BodyColor)] public string BodyColor { get; init; } = Var(Css.Tokens.Color.OnSurfaceVariant);
}

using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for link - component-specific geometry read by link.css.</summary>
public sealed record LinkTokens
{
    /// <summary>Focus Ring Width.</summary>
    [CssVar(LinkField.FocusRingWidth)] public required string FocusRingWidth { get; init; }

    /// <summary>Hover Opacity.</summary>
    [CssVar(LinkField.HoverOpacity)] public required string HoverOpacity { get; init; }

    /// <summary>How far a disabled link fades. A language that repaints disabled controls in a flat
    /// palette leaves this opaque and carries the change in its own stylesheet, since a foreground
    /// colour has no value meaning "leave this as painted".</summary>
    [CssVar(LinkField.DisabledOpacity)] public required string DisabledOpacity { get; init; }
}

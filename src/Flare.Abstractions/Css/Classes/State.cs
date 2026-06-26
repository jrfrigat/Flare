namespace Flare.Css.Classes;

/// <summary>
/// Reusable state-layer utility classes. They paint a translucent <c>::before</c> overlay on
/// hover, focus-visible, pressed or dragged using the <c>--flare-state-*-opacity</c> tokens, and
/// dim disabled elements. Opt-in: apply only the state(s) you need; each class is self-contained.
/// </summary>
public static class State
{
    /// <summary>The <c>flare-state-layer-hover</c> CSS class (overlay on hover).</summary>
    public const string LayerHover = "flare-state-layer-hover";
    /// <summary>The <c>flare-state-layer-focus</c> CSS class (overlay on <c>:focus-visible</c>).</summary>
    public const string LayerFocus = "flare-state-layer-focus";
    /// <summary>The <c>flare-state-layer-pressed</c> CSS class (overlay while <c>:active</c>).</summary>
    public const string LayerPressed = "flare-state-layer-pressed";
    /// <summary>The <c>flare-state-layer-dragged</c> CSS class (overlay while <c>aria-grabbed="true"</c>).</summary>
    public const string LayerDragged = "flare-state-layer-dragged";
    /// <summary>The <c>flare-state-disabled</c> CSS class (dims the element and blocks pointer events).</summary>
    public const string Disabled = "flare-state-disabled";
    /// <summary>The <c>flare-state-disabled-container</c> CSS class (lower-emphasis disabled container).</summary>
    public const string DisabledContainer = "flare-state-disabled-container";
}

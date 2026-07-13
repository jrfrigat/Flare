namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for state.</summary>
public static class State
{
    /// <summary>CSS custom-property name for the hover opacity token.</summary>
    public const string HoverOpacity = "--flare-state-hover-opacity";
    /// <summary>CSS custom-property name for the selected/active-container opacity token.</summary>
    public const string SelectedOpacity = "--flare-state-selected-opacity";
    /// <summary>CSS custom-property name for the focus opacity token.</summary>
    public const string FocusOpacity = "--flare-state-focus-opacity";
    /// <summary>CSS custom-property name for the pressed opacity token.</summary>
    public const string PressedOpacity = "--flare-state-pressed-opacity";
    /// <summary>CSS custom-property name for the dragged opacity token.</summary>
    public const string DraggedOpacity = "--flare-state-dragged-opacity";
    /// <summary>CSS custom-property name for the disabled opacity token.</summary>
    public const string DisabledOpacity = "--flare-state-disabled-opacity";
    /// <summary>CSS custom-property name for the disabled container opacity token.</summary>
    public const string DisabledContainerOpacity = "--flare-state-disabled-container-opacity";

    // State-layer PAINT tokens: the full background value (colour incl. alpha) of the hover/focus/pressed/
    // dragged overlay, so a theme chooses the state MODEL - a translucent currentColor wash, a discrete
    // flat fill, whatever - not just its strength. The core reads these; it bakes no state colour of its own.
    /// <summary>CSS custom-property name for the hover state-layer paint token.</summary>
    public const string HoverLayer = "--flare-state-hover-layer";
    /// <summary>CSS custom-property name for the focus state-layer paint token.</summary>
    public const string FocusLayer = "--flare-state-focus-layer";
    /// <summary>CSS custom-property name for the pressed state-layer paint token.</summary>
    public const string PressedLayer = "--flare-state-pressed-layer";
    /// <summary>CSS custom-property name for the dragged state-layer paint token.</summary>
    public const string DraggedLayer = "--flare-state-dragged-layer";
}

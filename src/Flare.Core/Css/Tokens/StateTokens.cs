namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for state.</summary>
public static class State
{
    private const string Prefix = $"{Vars.Flare}-state";

    /// <summary>CSS custom-property name for the hover opacity token.</summary>
    public const string HoverOpacity = $"{Prefix}-hover-opacity";
    /// <summary>CSS custom-property name for the focus opacity token.</summary>
    public const string FocusOpacity = $"{Prefix}-focus-opacity";
    /// <summary>CSS custom-property name for the pressed opacity token.</summary>
    public const string PressedOpacity = $"{Prefix}-pressed-opacity";
    /// <summary>CSS custom-property name for the dragged opacity token.</summary>
    public const string DraggedOpacity = $"{Prefix}-dragged-opacity";
    /// <summary>CSS custom-property name for the disabled opacity token.</summary>
    public const string DisabledOpacity = $"{Prefix}-disabled-opacity";
    /// <summary>CSS custom-property name for the disabled container opacity token.</summary>
    public const string DisabledContainerOpacity = $"{Prefix}-disabled-container-opacity";
}

namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for checkbox.</summary>
public static class Checkbox
{
    private const string FlareCheckbox = $"{Vars.Flare}-checkbox";

    /// <summary>CSS custom-property name for the border width token.</summary>
    public const string BorderWidth = $"{FlareCheckbox}-border-width";
    /// <summary>CSS custom-property name for the radius token.</summary>
    public const string Radius = $"{FlareCheckbox}-radius";
    /// <summary>CSS custom-property name for the state layer hover token.</summary>
    public const string StateLayerHover = $"{FlareCheckbox}-state-layer-hover";
    /// <summary>CSS custom-property name for the state layer hover checked token.</summary>
    public const string StateLayerHoverChecked = $"{FlareCheckbox}-state-layer-hover-checked";
    /// <summary>CSS custom-property name for the focus outline token.</summary>
    public const string FocusOutline = $"{FlareCheckbox}-focus-outline";
    /// <summary>CSS custom-property name for the focus outline offset token.</summary>
    public const string FocusOutlineOffset = $"{FlareCheckbox}-focus-outline-offset";
    /// <summary>CSS custom-property name for the focus shadow token.</summary>
    public const string FocusShadow = $"{FlareCheckbox}-focus-shadow";
}

/// <summary>CSS variable tokens for radio.</summary>
public static class Radio
{
    private const string FlareRadio = $"{Vars.Flare}-radio";

    /// <summary>CSS custom-property name for the state layer hover token.</summary>
    public const string StateLayerHover = $"{FlareRadio}-state-layer-hover";
    /// <summary>CSS custom-property name for the state layer hover checked token.</summary>
    public const string StateLayerHoverChecked = $"{FlareRadio}-state-layer-hover-checked";
}

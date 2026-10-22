namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for alert.</summary>
public static class Alert
{
    /// <summary>CSS custom-property name for the body opacity token.</summary>
    public const string BodyOpacity = "--flare-alert-body-opacity";
    /// <summary>CSS custom-property name for the close opacity token.</summary>
    public const string CloseOpacity = "--flare-alert-close-opacity";
    /// <summary>Border radius of the alert container.</summary>
    public const string Radius = "--flare-alert-radius";

    /// <summary>Border width (used for outlined / text variants).</summary>
    public const string BorderWidth = "--flare-alert-border-width";

    /// <summary>Internal padding of the alert container.</summary>
    public const string Padding = "--flare-alert-padding";

    /// <summary>Gap between icon and content.</summary>
    public const string Gap = "--flare-alert-gap";

    // Severity surfaces. Each role has a fill, a text colour and the outline the Outlined variant draws;
    // the component keeps no severity colour of its own, so a theme can repaint all four at once.

    /// <summary>Fill of an info alert.</summary>
    public const string InfoBg = "--flare-alert-info-bg";
    /// <summary>Text and icon colour of an info alert.</summary>
    public const string InfoColor = "--flare-alert-info-color";
    /// <summary>Outline of an outlined info alert.</summary>
    public const string InfoBorder = "--flare-alert-info-border";
    /// <summary>Fill of a success alert.</summary>
    public const string SuccessBg = "--flare-alert-success-bg";
    /// <summary>Text and icon colour of a success alert.</summary>
    public const string SuccessColor = "--flare-alert-success-color";
    /// <summary>Outline of an outlined success alert.</summary>
    public const string SuccessBorder = "--flare-alert-success-border";
    /// <summary>Fill of a warning alert.</summary>
    public const string WarningBg = "--flare-alert-warning-bg";
    /// <summary>Text and icon colour of a warning alert.</summary>
    public const string WarningColor = "--flare-alert-warning-color";
    /// <summary>Outline of an outlined warning alert.</summary>
    public const string WarningBorder = "--flare-alert-warning-border";
    /// <summary>Fill of an error alert.</summary>
    public const string ErrorBg = "--flare-alert-error-bg";
    /// <summary>Text and icon colour of an error alert.</summary>
    public const string ErrorColor = "--flare-alert-error-color";
    /// <summary>Outline of an outlined error alert.</summary>
    public const string ErrorBorder = "--flare-alert-error-border";
}

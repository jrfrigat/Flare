namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for alert.</summary>
public static class Alert
{
    private const string FlareAlert = $"{Vars.Flare}-alert";

    /// <summary>Border radius of the alert container.</summary>
    public const string Radius = $"{FlareAlert}-radius";

    /// <summary>Border width (used for outlined / text variants).</summary>
    public const string BorderWidth = $"{FlareAlert}-border-width";

    /// <summary>Internal padding of the alert container.</summary>
    public const string Padding = $"{FlareAlert}-padding";

    /// <summary>Gap between icon and content.</summary>
    public const string Gap = $"{FlareAlert}-gap";
}

namespace Flare.Theme.VisualStudio;

/// <summary>
/// Visual Studio theme-private CSS custom-property names (focus glow + the IDE tab strip). These are the
/// theme's OWN tokens (not part of the Flare core contract), so they live in the theme project - the core
/// stays theme-agnostic. The scoped VS CSS reads them and <see cref="VisualStudioTokens"/> assigns values.
/// </summary>
public static class VisualStudioCssVars
{
    /// <summary>Focus glow color.</summary>
    public const string Focus = "--flare-vs-focus";
    /// <summary>Tab strip background.</summary>
    public const string TabStripBg = "--flare-vs-tab-strip-bg";
    /// <summary>Gap between tabs.</summary>
    public const string TabGap = "--flare-vs-tab-gap";
    /// <summary>Tab corner radius.</summary>
    public const string TabRadius = "--flare-vs-tab-radius";
    /// <summary>Active tab background.</summary>
    public const string TabActiveBg = "--flare-vs-tab-active-bg";
}

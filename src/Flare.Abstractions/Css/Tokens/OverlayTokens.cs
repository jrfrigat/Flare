namespace Flare.Css.Tokens;

/// <summary>
/// CSS variable tokens shared by every surface that has to fit the screen - a dialog, a filter menu, a
/// shortcuts panel. The measurements are the same question asked in three places: how much air stays
/// between the surface and the edge, and how much of the screen a floating panel may take.
/// </summary>
public static class Overlay
{
    /// <summary>CSS custom-property name for the air left between a full-screen surface and the
    /// viewport edge.</summary>
    public const string ViewportInset = "--flare-overlay-viewport-inset";

    /// <summary>CSS custom-property name for the same air on a narrow screen, where a phone cannot
    /// spare as much of it.</summary>
    public const string ViewportInsetCompact = "--flare-overlay-viewport-inset-compact";

    /// <summary>CSS custom-property name for the share of the viewport a floating panel may grow to.</summary>
    public const string PanelMaxBlockSize = "--flare-overlay-panel-max-block-size";
}

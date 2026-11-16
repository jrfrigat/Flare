namespace Flare.Components.IDE;

/// <summary>
/// How <see cref="FlareIdeLayout"/> draws the caption of a side dock strip - the control above the left
/// or right tool region that returns to the tools while a docked document covers them. The Center strip
/// is configured separately with <see cref="FlareIdeLayout.ShowCenterDockStrip"/> and
/// <see cref="FlareIdeLayout.CenterDockHeader"/>.
/// </summary>
public enum IdeDockCaptionMode
{
    /// <summary>The caption is the button's visible text. This is the default.</summary>
    Text,

    /// <summary>The button shows only an icon; the localized region name stays its accessible name.</summary>
    IconOnly,

    /// <summary>The caption is not painted. The return-to-tools button keeps its localized accessible
    /// name and stays available to assistive technology while a docked document covers the region, and a
    /// side region without documents draws no strip at all, so no empty row is painted.</summary>
    Hidden,
}

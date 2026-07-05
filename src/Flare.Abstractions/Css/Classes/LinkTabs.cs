namespace Flare.Css.Classes;

/// <summary>CSS classes for <c>FlareLinkTabs</c> / <c>FlareLinkTab</c>.</summary>
public static class LinkTabs
{
    /// <summary>The <c>flare-link-tabs</c> CSS class.</summary>
    public const string Root = "flare-link-tabs";
    /// <summary>The <c>flare-link-tabs--tonal</c> CSS class.</summary>
    public const string Tonal = "flare-link-tabs--tonal";
    /// <summary>The <c>flare-link-tabs--filled</c> CSS class.</summary>
    public const string Filled = "flare-link-tabs--filled";

    // An item (FlareLinkTab) reuses FlareTabs' item classes directly - Css.Classes.Tabs.TabButton /
    // TabActive / TabDisabled / Label - so a route link is styled identically to an in-page tab and no
    // flare-link-tab* item classes are declared here. Only the bar container above is link-tabs-specific.
}

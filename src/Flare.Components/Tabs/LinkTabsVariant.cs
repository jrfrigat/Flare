namespace Flare.Components;

/// <summary>
/// Visual style of a <see cref="FlareLinkTabs"/> bar, independent of the active theme. Mirrors the
/// pill-shaped variants of <see cref="TabsVariant"/> (<see cref="Tonal"/>/<see cref="Filled"/>) so a
/// row of route links can read as the same segmented control as an in-page <see cref="FlareTabs"/>,
/// reusing the same <c>--flare-tabs-*</c> design tokens.
/// </summary>
public enum LinkTabsVariant
{
    /// <summary>The active theme's default tab style (a simple underline indicator).</summary>
    Default,

    /// <summary>Active link filled with a soft tonal pill (MD3 secondary-container, FluentUI2 "subtle-circular").</summary>
    Tonal,

    /// <summary>Active link filled with the solid accent as a pill / segmented control (MD3 primary-container, FluentUI2 "filled-circular").</summary>
    Filled,
}

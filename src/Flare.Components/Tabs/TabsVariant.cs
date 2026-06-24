namespace Flare.Components;

/// <summary>
/// Visual style of a <see cref="FlareTabs"/> bar, independent of the active theme. Each theme reskins
/// the variants through its tokens, so a variant looks native everywhere (e.g. <see cref="Filled"/>
/// is MD3 primary-container vs FluentUI2 "filled-circular"). Four names mirror <c>ButtonVariant</c>
/// (<see cref="Text"/>/<see cref="Tonal"/>/<see cref="Filled"/>/<see cref="Outlined"/>).
/// </summary>
public enum TabsVariant
{
    /// <summary>The active theme's default tab style (MD3 = secondary underline, FluentUI2 = transparent underline).</summary>
    Default,

    /// <summary>Full-width underline active indicator (MD3 secondary tabs, FluentUI2 "transparent").</summary>
    Underline,

    /// <summary>Content-hugging underline indicator with rounded top corners (MD3 primary tabs).</summary>
    Primary,

    /// <summary>No persistent indicator: the active tab is color/weight only, with a subtle hover wash (FluentUI2 "subtle").</summary>
    Text,

    /// <summary>Active tab filled with a soft tonal pill (MD3 secondary-container, FluentUI2 "subtle-circular").</summary>
    Tonal,

    /// <summary>Active tab filled with the solid accent as a pill / segmented control (MD3 primary-container, FluentUI2 "filled-circular").</summary>
    Filled,

    /// <summary>Bordered card / folder tabs that connect to the panel below (classic / Ant "card").</summary>
    Outlined,
}

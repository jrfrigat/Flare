namespace Flare.Components;

/// <summary>
/// Visual style of a <see cref="FlareTabs"/> bar, independent of the active theme. Each theme reskins
/// the variants through its tokens, so a variant looks native everywhere (e.g. <see cref="Filled"/>
/// is a solid accent pill). Four names mirror <c>ButtonVariant</c>
/// (<see cref="Text"/>/<see cref="Tonal"/>/<see cref="Filled"/>/<see cref="Outlined"/>).
/// </summary>
public enum TabsVariant
{
    /// <summary>The active theme's default tab style (typically a simple underline indicator).</summary>
    Default,

    /// <summary>Full-width underline active indicator.</summary>
    Underline,

    /// <summary>Content-hugging underline indicator with rounded top corners.</summary>
    Primary,

    /// <summary>No persistent indicator: the active tab is color/weight only, with a subtle hover wash.</summary>
    Text,

    /// <summary>Active tab filled with a soft tonal pill.</summary>
    Tonal,

    /// <summary>Active tab filled with the solid accent as a pill / segmented control.</summary>
    Filled,

    /// <summary>Bordered card / folder tabs that connect to the panel below (classic "card" tabs).</summary>
    Outlined,
}

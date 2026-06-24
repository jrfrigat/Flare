namespace Flare.Components;

/// <summary>
/// Visual style of a <see cref="FlareCard"/>, independent of the active theme. Each theme reskins the
/// variants through its <c>CardTokens</c>, so a variant looks native everywhere (e.g. <see cref="Tonal"/>
/// is MD3 secondary-container vs FluentUI2 "filled-alternative"). The five names mirror
/// <c>ButtonVariant</c> (<see cref="Filled"/>/<see cref="Outlined"/>/<see cref="Text"/>/<see cref="Elevated"/>/<see cref="Tonal"/>).
/// </summary>
public enum CardVariant
{
    /// <summary>Solid container tinted from the surface scale (MD3 filled card, FluentUI2 "filled").</summary>
    Filled,

    /// <summary>Bordered container with no elevation (MD3 outlined card, FluentUI2 "outline").</summary>
    Outlined,

    /// <summary>Transparent container with no border or shadow, only a hover wash (FluentUI2 "subtle").</summary>
    Text,

    /// <summary>Surface container lifted by a drop shadow (MD3 elevated card). The default.</summary>
    Elevated,

    /// <summary>Soft tonal container using the secondary-container color (MD3, FluentUI2 "filled-alternative").</summary>
    Tonal,
}

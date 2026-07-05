namespace Flare.Components;

/// <summary>
/// Visual style of a <see cref="FlareCard"/>, independent of the active theme. Each theme reskins the
/// variants through its <c>CardTokens</c>, so a variant looks native everywhere (e.g. <see cref="Tonal"/>
/// is a soft secondary-container fill). The five names mirror
/// <c>ButtonVariant</c> (<see cref="Filled"/>/<see cref="Outlined"/>/<see cref="Text"/>/<see cref="Elevated"/>/<see cref="Tonal"/>).
/// </summary>
public enum CardVariant
{
    /// <summary>Solid container tinted from the surface scale.</summary>
    Filled,

    /// <summary>Bordered container with no elevation.</summary>
    Outlined,

    /// <summary>Transparent container with no border or shadow, only a hover wash.</summary>
    Text,

    /// <summary>Surface container lifted by a drop shadow. The default.</summary>
    Elevated,

    /// <summary>Soft tonal container using the secondary-container color.</summary>
    Tonal,
}

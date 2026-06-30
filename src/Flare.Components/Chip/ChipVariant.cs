namespace Flare.Components;

/// <summary>
/// Visual style of a <see cref="FlareChip"/>, independent of the active theme. Mirrors the chip
/// styles in MD3: <see cref="Outlined"/> (the default - a bordered, transparent chip),
/// <see cref="Filled"/> (a solid surface fill with no border) and <see cref="Elevated"/>
/// (a filled surface lifted by a drop shadow).
/// </summary>
public enum ChipVariant
{
    /// <summary>Bordered chip with a transparent background (MD3 assist/filter chip). The default.</summary>
    Outlined,

    /// <summary>Solid surface fill with no border.</summary>
    Filled,

    /// <summary>Filled surface lifted by a level-1 drop shadow, with no border.</summary>
    Elevated,
}

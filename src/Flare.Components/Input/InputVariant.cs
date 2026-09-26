namespace Flare.Components;

/// <summary>
/// Visual variant of a text field (<c>FlareField</c>, <c>FlareNumericField</c>, <c>FlareTextArea</c>).
/// <see cref="Default"/> keeps the theme's own field style; <see cref="Filled"/> and <see cref="Outlined"/>
/// ask for the active theme's filled or outlined field, so the same variant follows each design language.
/// </summary>
public enum InputVariant
{
    /// <summary>Use the active theme's own field style.</summary>
    Default,
    /// <summary>The theme's filled field: a background fill, usually with a bottom indicator.</summary>
    Filled,
    /// <summary>The theme's outlined field: a stroke around the field, usually with no fill.</summary>
    Outlined,
    /// <summary>Bare: no container, border or indicator - only the text and its adornments, for a row
    /// whose surface the surrounding layout paints (an identifier strip, a toolbar search). Focus stays
    /// visible through the library's focus ring, and an error still shows.</summary>
    Bare,
}

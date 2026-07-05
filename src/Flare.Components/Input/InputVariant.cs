namespace Flare.Components;

/// <summary>
/// Visual variant of a text field (<c>FlareField</c>, <c>FlareNumericField</c>, <c>FlareTextArea</c>),
/// independent of the active theme. <see cref="Default"/> keeps the theme's own field style.
/// </summary>
public enum InputVariant
{
    /// <summary>Use the active theme's own field style.</summary>
    Default,
    /// <summary>Filled: surface container background + bottom active indicator.</summary>
    Filled,
    /// <summary>Outlined: transparent background + full outline, brand outline on focus.</summary>
    Outlined,
}

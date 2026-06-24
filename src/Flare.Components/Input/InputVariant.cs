namespace Flare.Components;

/// <summary>
/// Visual variant of a text field (<c>FlareField</c>, <c>FlareNumericField</c>, <c>FlareTextArea</c>),
/// independent of the active theme. <see cref="Default"/> keeps the theme's style
/// (MD3 = filled, Fluent = outlined).
/// </summary>
public enum InputVariant
{
    /// <summary>Use the active theme's field style (MD3 filled / Fluent outlined).</summary>
    Default,
    /// <summary>Filled: surface container background + bottom active indicator (MD3 filled-text-field).</summary>
    Filled,
    /// <summary>Outlined: transparent background + full 1dp outline, brand outline on focus (MD3 outlined-text-field).</summary>
    Outlined,
}

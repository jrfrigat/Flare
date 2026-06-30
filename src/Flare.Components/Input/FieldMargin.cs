namespace Flare.Components;

/// <summary>
/// Vertical (block) margin applied around a text field (<c>FlareField</c> / <c>FlareTextField</c>),
/// mirroring the dense/normal spacing presets common in form layouts.
/// </summary>
public enum FieldMargin
{
    /// <summary>No outer margin (the default - spacing is left to the surrounding layout).</summary>
    None,
    /// <summary>A small vertical margin above and below the field (compact forms).</summary>
    Dense,
    /// <summary>A normal vertical margin above and below the field (comfortable forms).</summary>
    Normal,
}

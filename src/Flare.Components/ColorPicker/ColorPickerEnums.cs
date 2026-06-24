namespace Flare.Components;

/// <summary>Output format for the color value produced by <see cref="FlareColorPicker"/>.</summary>
public enum ColorFormat
{
    /// <summary>Hexadecimal string, e.g. <c>#RRGGBB</c> or <c>#RRGGBBAA</c>.</summary>
    Hex,
    /// <summary>CSS <c>rgb()</c> / <c>rgba()</c> string.</summary>
    Rgb,
    /// <summary>CSS <c>hsl()</c> / <c>hsla()</c> string.</summary>
    Hsl,
}

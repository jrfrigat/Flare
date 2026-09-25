namespace Flare.Css;

/// <summary>
/// Declares which CSS custom-property NAME a design-token VALUE property populates, linking the
/// two token systems: the value records under <c>Flare.Abstractions.Tokens(.Components)</c> (per-theme
/// settings) and the name constants under <c>Flare.Css.Tokens.*</c> (the <c>--flare-*</c> registry).
/// <para>
/// The attribute IS the mapping: a source generator in Flare.Theming reads it off every string token property
/// reachable from <c>DesignTokens</c> and writes the assignment into <c>CssVarMap.FlattenDesign</c>, so a new
/// token reaches the browser as soon as its record property is declared - there is no table row to forget. Two
/// properties naming the same variable fail the build.
/// Apply it to scalar string token properties (one var name each). Compound tokens that expand to
/// several variables (per-corner radii, typography styles) are intentionally left unannotated and are
/// mapped by hand in <c>CssVarMap</c>.
/// </para>
/// </summary>
/// <param name="name">The CSS custom-property name (an <c>--flare-*</c> string, typically a
/// <c>Flare.Css.Tokens.*</c> constant) this property's value is written to.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class CssVarAttribute(string name) : Attribute
{
    /// <summary>The CSS custom-property name this token value populates, e.g. <c>--flare-btn-gap-xs</c>.</summary>
    public string Name { get; } = name;
}

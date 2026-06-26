namespace Flare.Css;

/// <summary>
/// Declares which CSS custom-property NAME a design-token VALUE property populates, linking the
/// two token systems: the value records under <c>Flare.Abstractions.Tokens(.Components)</c> (per-theme
/// settings) and the name constants under <c>Flare.Css.Tokens.*</c> (the <c>--flare-*</c> registry).
/// <para>
/// The mapping is otherwise expressed only imperatively in <c>CssVarMap.FlattenDesign</c>; this
/// attribute makes it declarative and self-documenting, and the <c>CssVar</c> drift test asserts that
/// every annotated name is actually emitted by the flatten - so the value/name systems cannot diverge.
/// Apply it to scalar string token properties (one var name each). Compound tokens that expand to
/// several variables (per-corner radii, typography styles) are intentionally left unannotated.
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

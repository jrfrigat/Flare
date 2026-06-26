using System.Reflection;
using Flare.Core.Services;        // FlattenDesign (CssVarMap) extension
using Flare.Core.Tokens.Components;
using Flare.Css;

namespace Flare.Core.Tests;

/// <summary>
/// Drift guard for the two token systems: every design-token VALUE property annotated with
/// <see cref="CssVarAttribute"/> must name a CSS variable that <c>CssVarMap.FlattenDesign</c> actually
/// emits. This keeps the declarative name-to-value link (the attributes) from silently diverging from
/// the imperative flatten - rename or drop a <c>Flare.Css.Tokens.*</c> constant and this test fails.
/// </summary>
public sealed class CssVarAttributeTests
{
    [Fact]
    public void EveryCssVarAttributeName_IsEmittedByFlattenDesign()
    {
        var emitted = TokenParityTests.CreateDefaultDesignTokens().FlattenDesign().Keys.ToHashSet();

        var annotated = typeof(ButtonTokens).Assembly.GetTypes()
            .SelectMany(t => t.GetProperties())
            .Select(p => (Property: p, Attr: p.GetCustomAttribute<CssVarAttribute>()))
            .Where(x => x.Attr is not null)
            .ToList();

        // Guard: the attribute is actually applied somewhere (otherwise this test would vacuously pass).
        Assert.NotEmpty(annotated);

        var missing = annotated
            .Where(x => !emitted.Contains(x.Attr!.Name))
            .Select(x => $"{x.Property.DeclaringType!.Name}.{x.Property.Name} -> '{x.Attr!.Name}'")
            .ToList();

        Assert.True(missing.Count == 0,
            "[CssVar] names not emitted by CssVarMap.FlattenDesign (token name/value drift):\n  " +
            string.Join("\n  ", missing));
    }
}

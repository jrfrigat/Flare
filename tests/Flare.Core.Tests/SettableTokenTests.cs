using System.Reflection;

namespace Flare.Core.Tests;

/// <summary>
/// Drift guard for the direction no other test covers: every <c>Flare.Css.Tokens</c> constant must name a
/// variable some theme can actually set - one that <c>CssVarMap.FlattenDesign</c> emits.
/// <para>
/// <see cref="CssVarAttributeTests"/> checks the opposite way round, that every <c>[CssVar]</c> reaches the
/// flatten, which cannot see a registered NAME that no token-record member stands behind. That gap is not
/// theoretical: <c>FlareSplitter</c> shipped seven such constants, so no theme could restyle it and every
/// value came from the core stylesheet - the token mandate inverted - and neither the CSS audit nor any
/// test noticed. A constant is a promise that the theme owns the value; this keeps the promise honest.
/// </para>
/// </summary>
public sealed class SettableTokenTests
{
    /// <summary>
    /// The only constants allowed to name a variable no theme emits, each for a reason that makes a theme
    /// value impossible rather than merely absent. Keep this list short and argued - a token that is simply
    /// missing its record member belongs in the fix, not here.
    /// </summary>
    private static readonly Dictionary<string, string> CoreOwned = new(StringComparer.Ordinal)
    {
        // A media query cannot read a custom property, so the real breakpoints are the literal px in the
        // @media rules of responsive.css/grid.css. These vars only republish those numbers so consumer CSS
        // and the C# FlareBreakpoints scale agree; a theme could not drive layout through them if it tried.
        [Css.Tokens.Breakpoint.Xs] = "read-only mirror of an @media boundary",
        [Css.Tokens.Breakpoint.Sm] = "read-only mirror of an @media boundary",
        [Css.Tokens.Breakpoint.Md] = "read-only mirror of an @media boundary",
        [Css.Tokens.Breakpoint.Lg] = "read-only mirror of an @media boundary",
        [Css.Tokens.Breakpoint.Xl] = "read-only mirror of an @media boundary",
        [Css.Tokens.Breakpoint.Xxl] = "read-only mirror of an @media boundary",
    };

    /// <summary>
    /// Stems of a runtime-composed family: the constant is a prefix and the theme emits names extending it
    /// (<c>--flare-btn-label</c> -> <c>--flare-btn-label-md-font</c>), so the stem itself is never emitted.
    /// Unlike <see cref="CoreOwned"/> this exemption validates itself - it only holds while the theme really
    /// emits members of the family, so deleting the family still fails the guard.
    /// </summary>
    private static readonly string[] RuntimeFamilyStems = ["--flare-btn-label"];

    private static bool IsLiveFamilyStem(string name, IReadOnlyCollection<string> emitted) =>
        RuntimeFamilyStems.Contains(name, StringComparer.Ordinal)
        && emitted.Any(e => e.StartsWith(name + "-", StringComparison.Ordinal));

    /// <summary>
    /// Per-instance channels: a component writes one on its own element and its own CSS reads it back -
    /// the angle of one clock hand, the column span of one grid cell, the indent of one tree row. A theme
    /// cannot set them, and it would be meaningless if it could, so the whole type is exempt rather than
    /// each name being listed.
    /// </summary>
    /// <remarks>
    /// Exempting a type rather than a list of names is the point: a new channel is covered the moment it
    /// is declared in the right place, and putting a real design token there by mistake is caught by
    /// <see cref="NoLocalVar_IsEmittedByATheme"/> below.
    /// </remarks>
    private const string LocalVarsOwner = nameof(Flare.Css.Tokens.LocalVars);

    private static List<(string Owner, string Name)> TokenConstants() =>
        typeof(Flare.Css.Tokens.Splitter).Assembly.GetTypes()
            .Where(t => t.Namespace == "Flare.Css.Tokens")
            .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            .Where(f => f is { IsLiteral: true, IsInitOnly: false } && f.FieldType == typeof(string))
            .Select(f => (Owner: $"{f.DeclaringType!.Name}.{f.Name}", Name: (string)f.GetRawConstantValue()!))
            .Where(x => x.Name.StartsWith("--flare-", StringComparison.Ordinal))
            .ToList();

    [Fact]
    public void EveryTokenConstant_IsSettableByATheme()
    {
        var emitted = TokenParityTests.ThemeEmittedTokenNames();
        var constants = TokenConstants();

        // Guard against a reflection change quietly emptying the set and making this pass vacuously.
        Assert.True(constants.Count > 300, $"Expected 300+ token constants, found {constants.Count}.");

        var unsettable = constants
            .Where(x => !emitted.Contains(x.Name)
                     && !CoreOwned.ContainsKey(x.Name)
                     && !x.Owner.StartsWith(LocalVarsOwner + ".", StringComparison.Ordinal)
                     && !IsLiveFamilyStem(x.Name, emitted))
            .Select(x => $"{x.Owner} -> '{x.Name}'")
            .Order(StringComparer.Ordinal)
            .ToList();

        Assert.True(unsettable.Count == 0,
            "Css.Tokens constants that no theme can set, so the value can only come from core CSS - which\n" +
            "is the token mandate inverted. Add a `required` token-record member plus a CssVarMap mapping,\n" +
            "or delete the constant. Only add to CoreOwned when a theme value is impossible, not absent:\n  " +
            string.Join("\n  ", unsettable));
    }

    /// <summary>
    /// Keeps <see cref="CoreOwned"/> from rotting: once a token becomes theme-settable its exemption is a
    /// lie that would hide the next regression on that name.
    /// </summary>
    [Fact]
    public void NoCoreOwnedException_IsStale()
    {
        var emitted = TokenParityTests.ThemeEmittedTokenNames();
        var declared = TokenConstants().Select(x => x.Name).ToHashSet(StringComparer.Ordinal);

        var stale = CoreOwned.Keys
            .Where(name => emitted.Contains(name) || !declared.Contains(name))
            .Select(name => emitted.Contains(name)
                ? $"'{name}' is emitted by a theme now - drop the exemption"
                : $"'{name}' is no longer a Css.Tokens constant - drop the exemption")
            .Order(StringComparer.Ordinal)
            .ToList();

        Assert.True(stale.Count == 0, "Stale CoreOwned exemptions:\n  " + string.Join("\n  ", stale));
    }

    /// <summary>
    /// Keeps the <c>LocalVars</c> type exemption honest: a name in there that a theme emits is not a
    /// per-instance channel at all, and the whole-type exemption would be hiding a real token from the
    /// settable-token guard.
    /// </summary>
    [Fact]
    public void NoLocalVar_IsEmittedByATheme()
    {
        var emitted = TokenParityTests.ThemeEmittedTokenNames();

        var misfiled = TokenConstants()
            .Where(x => x.Owner.StartsWith(LocalVarsOwner + ".", StringComparison.Ordinal))
            .Where(x => emitted.Contains(x.Name))
            .Select(x => $"{x.Owner} -> '{x.Name}'")
            .Order(StringComparer.Ordinal)
            .ToList();

        Assert.True(misfiled.Count == 0,
            "These sit in LocalVars, which exempts them from the settable-token guard, but a theme emits\n" +
            "them - so they are design tokens filed as per-instance channels. Move them to the component's\n" +
            "own token class and give them a record member:\n  " + string.Join("\n  ", misfiled));
    }
}

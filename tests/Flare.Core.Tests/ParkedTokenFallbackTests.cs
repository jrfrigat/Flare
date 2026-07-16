using System.Reflection;
using Flare.Abstractions;
using Flare.Theme.Aero;
using Flare.Theme.FluentUI2;
using Flare.Theme.LiquidGlass;
using Flare.Theme.MaterialDesign2;
using Flare.Theme.MaterialDesign3;
using Flare.Theme.MaterialDesign3Expressive;
using Flare.Theme.VisualStudio;
using Flare.Theming;

namespace Flare.Core.Tests;

/// <summary>
/// Enforces the token mandate at its weakest point: <b>a theme must supply a real value for every token</b>,
/// because the component CSS is not allowed to carry defaults of its own.
///
/// The tempting shortcut is to "park" a token at <c>initial</c>, meaning "I do not override this - use the
/// component's own default". It even looks harmless, because <c>initial</c> is the guaranteed-invalid value
/// for a custom property, so <c>var(--token, &lt;fallback&gt;)</c> skips it and takes the fallback. But that
/// only works if the component CSS holds the value - i.e. the theme has pushed its own opinion into the core,
/// which is exactly what the mandate forbids. And the moment someone strips those fallbacks as "dead code"
/// (they look dead: every theme emits the token), the substitution yields nothing, the declaration is invalid
/// at computed-value time, and the geometry silently disappears.
///
/// That is not hypothetical: it shipped in v0.2.0-v0.3.0. The slider rail collapsed to 0px at every size,
/// FlarePagination lost its button ramp and FlareRating lost its star ramp - under MaterialDesign3, the
/// default theme. Name-level auditing (CssAudit) cannot see it, because every name is present and in sync.
///
/// Size-dependent geometry is what drove themes to park in the first place: a single <c>:root</c> token
/// cannot hold five per-size values. The answer is one token per size (see <c>SliderTokens</c>,
/// <c>RatingTokens</c>, <c>PaginationTokens</c>, and <c>ButtonTokens</c> before them) - the theme emits all
/// five and the component's size class only selects which to read.
///
/// Together with <see cref="DeadFallbackTests"/> this makes "no defaults in the core" enforceable: no theme
/// may park a value, and therefore no core fallback may exist for a token a theme supplies.
/// </summary>
public sealed class ParkedTokenFallbackTests
{
    // Every in-box theme. Kept explicit so the theory names read well; EveryInBoxTheme_IsCovered below
    // reflects over the theme assemblies and fails if a new theme is not added here.
    private static readonly Func<ITheme>[] Factories =
    [
        () => new AeroTheme(),
        () => new FluentUI2Theme(),
        () => new LiquidGlassTheme(),
        () => new MaterialDesign2Theme(),
        () => new MaterialDesign3Theme(),
        () => new MaterialDesign3ExpressiveTheme(),                 // Material Design 3 Expressive
        () => new VisualStudioTheme(),
    ];

    public static TheoryData<int> ThemeIndexes()
    {
        var d = new TheoryData<int>();
        for (var i = 0; i < Factories.Length; i++) d.Add(i);
        return d;
    }

    // `initial` on a custom property is guaranteed-invalid, so it never reaches the page as a value - it only
    // makes var() fall back. An empty value is not emitted at all. Either way the theme supplied nothing.
    private static bool IsParked(string? value) =>
        string.IsNullOrWhiteSpace(value) || value.Trim() == "initial";

    [Theory]
    [MemberData(nameof(ThemeIndexes))]
    public void InBoxTheme_SuppliesARealValueForEveryToken(int index)
    {
        var theme = Factories[index]();

        var parked = theme.Design.FlattenDesign()
            .Where(kv => IsParked(kv.Value))
            .Select(kv => $"{kv.Key} = '{kv.Value}'")
            .OrderBy(s => s, StringComparer.Ordinal)
            .ToList();

        Assert.True(parked.Count == 0,
            $"The '{theme.Id}' theme parks these tokens instead of supplying a value, which forces the "
            + "component CSS to carry the default - the token mandate forbids that, and it silently breaks "
            + "the moment the fallback is stripped. Give the theme a real value; if the value is "
            + "size-dependent, add one token per size (see SliderTokens / ButtonTokens) instead of "
            + "parking:\n  "
            + string.Join("\n  ", parked));
    }

    /// <summary>A new in-box theme must be added to <see cref="Factories"/>, or it escapes the guard above.</summary>
    [Fact]
    public void EveryInBoxTheme_IsCovered()
    {
        var covered = Factories.Select(f => f().GetType()).ToHashSet();

        var all = new[]
            {
                typeof(AeroTheme), typeof(FluentUI2Theme), typeof(LiquidGlassTheme), typeof(MaterialDesign2Theme),
                typeof(MaterialDesign3Theme), typeof(MaterialDesign3ExpressiveTheme), typeof(VisualStudioTheme),
            }
            .Select(t => t.Assembly).Distinct()
            .SelectMany(SafeTypes)
            .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic
                        && typeof(ITheme).IsAssignableFrom(t)
                        && t.GetConstructor(Type.EmptyTypes) is not null)
            .ToList();

        var missing = all.Where(t => !covered.Contains(t)).Select(t => t.FullName).ToList();
        Assert.True(missing.Count == 0,
            "These in-box themes are not covered by the parked-token guard - add them to Factories:\n  "
            + string.Join("\n  ", missing));
    }

    private static IEnumerable<Type> SafeTypes(Assembly a)
    {
        try { return a.GetTypes(); }
        catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t is not null)!; }
    }
}

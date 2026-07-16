using Flare.Theme.FluentUI2.Tokens;
using Flare.Theme.MaterialDesign3.Tokens;
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
/// </summary>
public sealed class ParkedTokenFallbackTests
{
    public static TheoryData<string> ReferenceThemes() => new() { "MaterialDesign3", "FluentUI2" };

    private static IReadOnlyDictionary<string, string> Flatten(string theme) => theme switch
    {
        "MaterialDesign3" => MaterialDesignTokens.Design.FlattenDesign(),
        "FluentUI2" => FluentUI2Tokens.Design.FlattenDesign(),
        _ => throw new ArgumentOutOfRangeException(nameof(theme), theme, "Unknown reference theme."),
    };

    // `initial` on a custom property is guaranteed-invalid, so it never reaches the page as a value - it only
    // makes var() fall back. An empty value is not emitted at all. Either way the theme supplied nothing.
    private static bool IsParked(string? value) =>
        string.IsNullOrWhiteSpace(value) || value.Trim() == "initial";

    [Theory]
    [MemberData(nameof(ReferenceThemes))]
    public void ReferenceTheme_SuppliesARealValueForEveryToken(string theme)
    {
        var parked = Flatten(theme)
            .Where(kv => IsParked(kv.Value))
            .Select(kv => $"{kv.Key} = '{kv.Value}'")
            .OrderBy(s => s, StringComparer.Ordinal)
            .ToList();

        Assert.True(parked.Count == 0,
            $"The {theme} theme parks these tokens instead of supplying a value, which forces the component "
            + "CSS to carry the default - the token mandate forbids that, and it silently breaks the moment "
            + "the fallback is stripped. Give the theme a real value; if the value is size-dependent, add one "
            + "token per size (see SliderTokens / ButtonTokens) instead of parking:\n  "
            + string.Join("\n  ", parked));
    }
}

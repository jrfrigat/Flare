using Flare.Css;
using Flare.Theming;

namespace Flare.Core.Tests;

public sealed class CssVarRecordTests
{
    private record PluginTokens
    {
        [CssVar("--example-surface")] public string Surface { get; init; } = Css.Tokens.Vars.Var(Css.Tokens.Color.Surface);
        [CssVar("--example-size")] public string? Size { get; init; } = Css.Tokens.Vars.Var(Css.Tokens.Spacing.S16);
        public string Unmapped { get; init; } = "Not a CSS variable";
    }
    private sealed record ExtendedPluginTokens : PluginTokens
    {
        [CssVar("--example-accent")] public string Accent { get; init; } = Css.Tokens.Vars.Var(Css.Tokens.Color.Primary);
    }
    private sealed record DuplicateTokens
    {
        [CssVar("--example-duplicate")] public string First { get; init; } = "a";
        [CssVar("--example-duplicate")] public string Second { get; init; } = "b";
    }

    [Fact]
    public void DefaultsReferenceBaseTokensAndValuesComeFromEachInstance()
    {
        var defaults = new PluginTokens();
        Assert.Equal(Css.Tokens.Vars.Var(Css.Tokens.Color.Surface), defaults.FlattenTokens()["--example-surface"]);
        var custom = defaults with { Surface = Css.Tokens.Vars.Var(Css.Tokens.Color.SecondaryContainer), Size = null };
        var map = custom.FlattenTokens();
        Assert.Single(map);
        Assert.Equal(custom.Surface, map["--example-surface"]);
        Assert.Equal(defaults.Surface, defaults.FlattenTokens()["--example-surface"]);
    }

    [Fact]
    public void ExternalDerivedRecordsKeepBaseAndCustomFields()
    {
        var map = new ExtendedPluginTokens().FlattenTokens();
        Assert.Equal(3, map.Count);
        Assert.Contains("--example-surface", map.Keys);
        Assert.Contains("--example-accent", map.Keys);
    }

    [Fact]
    public void AmbiguousVariableMappingsFailRatherThanSilentlyOverwrite()
    {
        var error = Assert.Throws<InvalidOperationException>(() => new DuplicateTokens().FlattenTokens());
        Assert.Contains("--example-duplicate", error.Message);
    }

    [Fact]
    public void IndependentRecordsUseTheExistingThemeExtensionPipeline()
    {
        var plugin = new PluginTokens { Surface = Css.Tokens.Vars.Var(Css.Tokens.Color.SecondaryContainer) };
        var design = TokenParityTests.CreateDefaultDesignTokens() with { Extended = plugin.FlattenTokens() };
        var result = design.FlattenDesign();
        Assert.Equal(plugin.Surface, result["--example-surface"]);
        Assert.Contains(Css.Tokens.Splitter.GutterSize, result.Keys);
    }
}

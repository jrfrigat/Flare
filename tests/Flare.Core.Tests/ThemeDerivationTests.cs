using Flare.Abstractions;
using Flare.Abstractions.Tokens;
using Flare.Theming;

namespace Flare.Core.Tests;

/// <summary>
/// Verifies <see cref="ThemeDerivation.Derive"/>: a derived theme forwards every member of its base
/// except the ones explicitly overridden, and applies the design transform once.
/// </summary>
public sealed class ThemeDerivationTests
{
    private sealed class BaseStub : ITheme
    {
        public string Id => "base";
        public string DisplayName => "Base";
        public DesignTokens Design { get; } = TokenParityTests.CreateDefaultDesignTokens();
        public string DefaultPaletteId => "base-pal";
        public IReadOnlyList<string> StyleAssets => ["base.css"];
        public IReadOnlyList<Palette> Palettes => [];
        public IPaletteGenerator? PaletteGenerator => null;
    }

    [Fact]
    public void Derive_OverridesOnlySpecified_AndForwardsTheRest()
    {
        var b = new BaseStub();

        var d = b.Derive(id: "derived", design: x => x with { FocusRing = "9px solid red" });

        Assert.Equal("derived", d.Id);                       // overridden
        Assert.Equal("Base", d.DisplayName);                 // forwarded
        Assert.Equal("base-pal", d.DefaultPaletteId);        // forwarded
        Assert.Equal(b.StyleAssets, d.StyleAssets);          // forwarded
        Assert.Equal("9px solid red", d.Design.FocusRing);   // transformed
        Assert.Equal(b.Design.Shape, d.Design.Shape);        // the rest of the design is preserved
    }

    [Fact]
    public void Derive_NullDesign_KeepsBaseDesignInstance()
    {
        var b = new BaseStub();
        var d = b.Derive(id: "derived");
        Assert.Same(b.Design, d.Design);
    }

    [Fact]
    public void Derive_RequiresNonEmptyId()
    {
        var b = new BaseStub();
        Assert.Throws<ArgumentException>(() => b.Derive(id: ""));
    }
}

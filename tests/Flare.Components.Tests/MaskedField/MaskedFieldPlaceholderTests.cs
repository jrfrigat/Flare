namespace Flare.Components.Tests;

public class MaskedFieldPlaceholderTests : FlareTestContext
{
    private static string? Placeholder(IRenderedComponent<FlareMaskedField> cut)
        => cut.Find("input").GetAttribute("placeholder");

    [Fact]
    public void ExplicitPlaceholder_WinsOverTheDerivedOne()
    {
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Mask, "##/##/####")
            .Add(x => x.Placeholder, "DD/MM/YYYY"));

        Assert.Equal("DD/MM/YYYY", Placeholder(cut));
    }

    [Fact]
    public void ExplicitPlaceholder_WinsOverAPreset()
    {
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Preset, MaskPreset.Date)
            .Add(x => x.Placeholder, "DD/MM/YYYY"));

        Assert.Equal("DD/MM/YYYY", Placeholder(cut));
    }

    [Fact]
    public void EmptyPlaceholder_UsesTheDerivedOne()
    {
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Mask, "##/##/####")
            .Add(x => x.Placeholder, string.Empty));

        Assert.Equal("__/__/____", Placeholder(cut));
    }

    [Fact]
    public void DerivedPlaceholder_FollowsTheMask()
    {
        var cut = Render<FlareMaskedField>(p => p.Add(x => x.Mask, "###"));
        Assert.Equal("___", Placeholder(cut));

        cut.Render(p => p.Add(x => x.Mask, "####"));

        Assert.Equal("____", Placeholder(cut));
    }

    [Fact]
    public void Preset_WithoutExplicitPlaceholder_DerivesFromThePattern()
    {
        var cut = Render<FlareMaskedField>(p => p.Add(x => x.Preset, MaskPreset.Date));

        Assert.Equal("__/__/____", Placeholder(cut));
    }
}

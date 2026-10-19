namespace Flare.Components.Tests;

public class MaskedFieldValueContractTests : FlareTestContext
{
    private static string Shown(IRenderedComponent<FlareMaskedField> cut)
        => cut.Find("input").GetAttribute("value") ?? string.Empty;

    [Fact]
    public void AppliedMask_RaisesTheFormattedValue()
    {
        string? bound = null;
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Mask, "###-###")
            .Add(x => x.ValueChanged, (string? v) => bound = v));

        cut.Find("input").Input("123456");

        Assert.Equal("123-456", Shown(cut));
        Assert.Equal("123-456", bound);
    }

    [Fact]
    public void Preset_RaisesTheFormattedValue()
    {
        string? bound = null;
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Preset, MaskPreset.Phone)
            .Add(x => x.ValueChanged, (string? v) => bound = v));

        cut.Find("input").Input("1234567890");

        Assert.Equal("(123) 456-7890", Shown(cut));
        Assert.Equal("(123) 456-7890", bound);
    }

    [Fact]
    public void FreeText_RaisesTheTypedText()
    {
        string? bound = null;
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Mask, "###-###")
            .Add(x => x.AutoApply, false)
            .Add(x => x.ValueChanged, (string? v) => bound = v));

        cut.Find("input").Input("12-34x");

        Assert.Equal("12-34x", Shown(cut));
        Assert.Equal("12-34x", bound);
    }

    [Fact]
    public void ParentEcho_KeepsTheFormattedValue()
    {
        string? bound = null;
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Mask, "###-###")
            .Add(x => x.ValueChanged, (string? v) => bound = v));

        cut.Find("input").Input("123456");
        cut.Render(p => p.Add(x => x.Value, bound));

        Assert.Equal("123-456", Shown(cut));
        Assert.Equal("123-456", bound);
    }

    [Fact]
    public void FormattedValue_IsTheInitialValueContract()
    {
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Mask, "###-###")
            .Add(x => x.Value, "123-456"));

        Assert.Equal("123-456", Shown(cut));
    }
}

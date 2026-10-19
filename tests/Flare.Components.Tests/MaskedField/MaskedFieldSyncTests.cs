namespace Flare.Components.Tests;

public class MaskedFieldSyncTests : FlareTestContext
{
    private static string Shown(IRenderedComponent<FlareMaskedField> cut)
        => cut.Find("input").GetAttribute("value") ?? string.Empty;

    [Fact]
    public void ExternalValue_SetAndClear_UpdatesDisplay()
    {
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Mask, "###-###")
            .Add(x => x.Value, "123456"));
        Assert.Equal("123-456", Shown(cut));

        cut.Render(p => p.Add(x => x.Value, null));
        Assert.Equal(string.Empty, Shown(cut));

        cut.Render(p => p.Add(x => x.Value, "654321"));
        Assert.Equal("654-321", Shown(cut));
    }

    [Fact]
    public void MaskChange_ReformatsTheCurrentValue()
    {
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Mask, "###-###")
            .Add(x => x.Value, "123456"));

        cut.Render(p => p.Add(x => x.Mask, "### ###"));

        Assert.Equal("123 456", Shown(cut));
    }

    [Fact]
    public void PresetChange_ReformatsTheCurrentValue()
    {
        var cut = Render<FlareMaskedField>(p => p.Add(x => x.Value, "1234567890"));

        cut.Render(p => p.Add(x => x.Preset, MaskPreset.Phone));

        Assert.Equal("(123) 456-7890", Shown(cut));
    }

    [Fact]
    public void AutoApplyChange_ReformatsTheCurrentValue()
    {
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Mask, "###-###")
            .Add(x => x.Value, "123456"));
        Assert.Equal("123-456", Shown(cut));

        cut.Render(p => p.Add(x => x.AutoApply, false));
        Assert.Equal("123456", Shown(cut));

        cut.Render(p => p.Add(x => x.AutoApply, true));
        Assert.Equal("123-456", Shown(cut));
    }

    [Fact]
    public void LocalInput_SurvivesAnUnchangedParentRender()
    {
        var cut = Render<FlareMaskedField>(p => p.Add(x => x.Mask, "###-###"));

        cut.Find("input").Input("123456");
        Assert.Equal("123-456", Shown(cut));

        cut.Render(p => p.Add(x => x.Mask, "###-###"));

        Assert.Equal("123-456", Shown(cut));
    }
}

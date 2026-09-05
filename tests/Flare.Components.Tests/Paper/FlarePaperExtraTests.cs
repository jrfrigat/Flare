namespace Flare.Components.Tests;

public class FlarePaperExtraTests : FlareTestContext
{
    [Fact]
    public void PaddingApplied_AsModifierClass()
    {
        var cut = Render<FlarePaper>(p => p
            .Add(x => x.Padding, FlareSpacing.Medium));

        Assert.Contains(Css.Classes.Paper.PaddingMedium, cut.Find($".{Css.Classes.Paper.Root}").ClassName);
    }

    [Fact]
    public void PaddingCustom_AppliedToStyle()
    {
        var cut = Render<FlarePaper>(p => p
            .Add(x => x.Padding, FlareSpacing.Custom)
            .Add(x => x.PaddingValue, "1rem"));

        var style = cut.Find($".{Css.Classes.Paper.Root}").GetAttribute("style") ?? string.Empty;
        Assert.Contains("padding:1rem", style);
    }

    [Fact]
    public void DefaultElevation_IsOne()
    {
        var cut = Render<FlarePaper>();

        Assert.Contains(Css.Classes.Paper.Elevation1, cut.Find($".{Css.Classes.Paper.Root}").ClassName);
    }
}

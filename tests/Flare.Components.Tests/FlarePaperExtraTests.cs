// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

public class FlarePaperExtraTests : FlareTestContext
{
    [Fact]
    public void PaddingApplied_AsModifierClass()
    {
        var cut = RenderComponent<FlarePaper>(p => p
            .Add(x => x.Padding, FlareSpacing.Medium));

        Assert.Contains("flare-paper--padding-medium", cut.Find(".flare-paper").ClassName);
    }

    [Fact]
    public void PaddingCustom_AppliedToStyle()
    {
        var cut = RenderComponent<FlarePaper>(p => p
            .Add(x => x.Padding, FlareSpacing.Custom)
            .Add(x => x.PaddingValue, "1rem"));

        var style = cut.Find(".flare-paper").GetAttribute("style") ?? string.Empty;
        Assert.Contains("padding:1rem", style);
    }

    [Fact]
    public void DefaultElevation_IsOne()
    {
        var cut = RenderComponent<FlarePaper>();

        Assert.Contains("flare-paper--elevation-1", cut.Find(".flare-paper").ClassName);
    }
}

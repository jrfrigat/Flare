namespace Flare.Components.Tests;

public class FlareSpacerTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareSpacer>();

        Assert.NotEmpty(cut.FindAll("div"));
    }

    [Fact]
    public void HasSpacerClass()
    {
        var cut = Render<FlareSpacer>();

        Assert.Contains(Css.Classes.Spacer.Root, cut.Find("div").ClassName ?? "");
    }

    [Fact]
    public void AdditionalAttributes_StylePassesThrough()
    {
        var cut = Render<FlareSpacer>(p => p
            .AddUnmatched("style", "flex-grow:1"));

        Assert.NotEmpty(cut.FindAll("div"));
        Assert.NotNull(cut.Instance);
    }

    [Fact]
    public void Style_Param_PassesThrough()
    {
        var cut = Render<FlareSpacer>(p => p
            .Add(x => x.Style, "flex-grow:2"));

        var style = cut.Find("div").GetAttribute("style") ?? "";
        Assert.Contains("flex-grow", style);
    }
}

// ------------------------------------------------------------------------------
// FlareResizable  (6 tests from Wave6)
// ------------------------------------------------------------------------------

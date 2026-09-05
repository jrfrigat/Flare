namespace Flare.Components.Tests;

public class FlareBadgeTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareBadge>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Badge.Root}"));
    }

    [Fact]
    public void RendersCount()
    {
        var cut = Render<FlareBadge>(p => p
            .Add(x => x.Count, 7));

        Assert.Contains("7", cut.Find($".{Css.Classes.Badge.Indicator}").TextContent);
    }

    [Fact]
    public void MaxCount_ShowsPlusNotation()
    {
        var cut = Render<FlareBadge>(p => p
            .Add(x => x.Count, 150)
            .Add(x => x.Max, 99));

        Assert.Contains("99+", cut.Find($".{Css.Classes.Badge.Indicator}").TextContent);
    }

    [Fact]
    public void DotVariant_RendersIndicatorWithDotClass()
    {
        var cut = Render<FlareBadge>(p => p
            .Add(x => x.Dot, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Badge.IndicatorDot}"));
    }

    [Fact]
    public void WrapsChildContent()
    {
        var cut = Render<FlareBadge>(p => p
            .AddChildContent("<span class=\"wrapped-item\">Icon</span>"));

        Assert.NotEmpty(cut.FindAll(".wrapped-item"));
    }

    [Fact]
    public void Text_OverridesCountLabel()
    {
        var cut = Render<FlareBadge>(p => p
            .Add(x => x.Text, "NEW")
            .Add(x => x.Count, 5));

        Assert.Contains("NEW", cut.Find($".{Css.Classes.Badge.Indicator}").TextContent);
    }

    [Fact]
    public void Standalone_AddsModifierClass()
    {
        var cut = Render<FlareBadge>(p => p
            .Add(x => x.Text, "Beta")
            .Add(x => x.Standalone, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Badge.Standalone}"));
    }

    [Fact]
    public void WithoutAnchor_IsStandaloneByDefault()
    {
        var cut = Render<FlareBadge>(p => p
            .Add(x => x.Text, "Tag"));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Badge.Standalone}"));
    }
}

// ------------------------------------------------------------------------------
// FlarePaper  (4 tests from Wave3)
// ------------------------------------------------------------------------------

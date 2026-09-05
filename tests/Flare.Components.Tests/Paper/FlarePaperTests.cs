namespace Flare.Components.Tests;

public class FlarePaperTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlarePaper>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Paper.Root}"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlarePaper>(p => p
            .AddChildContent("<p class=\"paper-content\">Hello</p>"));

        Assert.NotEmpty(cut.FindAll(".paper-content"));
    }

    [Fact]
    public void ElevationClass_AppliedCorrectly()
    {
        var cut = Render<FlarePaper>(p => p
            .Add(x => x.Elevation, 3));

        Assert.Contains(Css.Classes.Paper.Elevation3, cut.Find($".{Css.Classes.Paper.Root}").ClassName);
    }

    [Fact]
    public void SquarePaper_HasSquareClass()
    {
        var cut = Render<FlarePaper>(p => p
            .Add(x => x.Square, true));

        Assert.Contains(Css.Classes.Paper.Square, cut.Find($".{Css.Classes.Paper.Root}").ClassName);
    }
}

// ------------------------------------------------------------------------------
// FlareTimeline  (5 tests from Wave3)
// ------------------------------------------------------------------------------

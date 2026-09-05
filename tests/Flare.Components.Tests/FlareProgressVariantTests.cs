// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareProgress Buffer / Query  (8 tests)
// -----------------------------------------------------------------------------

public class FlareProgressVariantTests : FlareTestContext
{
    [Fact]
    public void Linear_RendersLinearClass()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Variant, ProgressVariant.Linear));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Progress.Linear}"));
    }

    [Fact]
    public void Buffer_RendersBufferClass()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Variant, ProgressVariant.Buffer));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Progress.Buffer}"));
    }

    [Fact]
    public void Query_RendersQueryClass()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Variant, ProgressVariant.Query));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Progress.Query}"));
    }

    [Fact]
    public void Buffer_WithBufferValue_RendersBufferFill()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Variant, ProgressVariant.Buffer)
            .Add(x => x.BufferValue, 60.0));

        var fill = cut.Find($".{Css.Classes.Progress.BufferFill}");
        var style = fill.GetAttribute("style") ?? "";
        Assert.Contains("60", style);
    }

    [Fact]
    public void Circular_RendersSvgElement()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Variant, ProgressVariant.Circular));

        Assert.NotEmpty(cut.FindAll("svg"));
    }

    [Fact]
    public void Linear_WithValue50_AppliesWidthStyle()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Variant, ProgressVariant.Linear)
            .Add(x => x.Value, 50.0));

        var bar = cut.Find($".{Css.Classes.Progress.Bar}");
        var style = bar.GetAttribute("style") ?? "";
        Assert.Contains("50", style);
    }

    [Fact]
    public void Buffer_RendersRootElement()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Variant, ProgressVariant.Buffer));

        Assert.NotEmpty(cut.FindAll("[role='progressbar']"));
    }

    [Fact]
    public void Query_RendersRootElement()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Variant, ProgressVariant.Query));

        Assert.NotEmpty(cut.FindAll("[role='progressbar']"));
    }
}

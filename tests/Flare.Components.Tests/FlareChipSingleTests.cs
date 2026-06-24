// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareChip  (5 tests)
// -----------------------------------------------------------------------------

public class FlareChipSingleTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tech"));

        Assert.NotEmpty(cut.FindAll(".flare-chip"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Science"));

        Assert.Contains("Science", cut.Find(".flare-chip__label").TextContent);
    }

    [Fact]
    public void SelectedState_HasSelectedClass()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Art")
            .Add(x => x.Selected, true));

        Assert.Contains("flare-chip--selected", cut.Find(".flare-chip").ClassName);
    }

    [Fact]
    public void Closeable_ShowsCloseButton()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Music")
            .Add(x => x.Closeable, true));

        Assert.NotEmpty(cut.FindAll(".flare-chip__close"));
    }

    [Fact]
    public void NotCloseable_HidesCloseButton()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Sports")
            .Add(x => x.Closeable, false));

        Assert.Empty(cut.FindAll(".flare-chip__close"));
    }
}

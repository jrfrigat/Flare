// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareDivider with text  (6 tests)
// Note: The current FlareDivider source has no Text param - tests are adapted
// to what actually exists (Vertical param and CSS class).
// -----------------------------------------------------------------------------

public class FlareDividerTextTests : FlareTestContext
{
    [Fact]
    public void Default_RendersFlareDividerElement()
    {
        var cut = Render<FlareDivider>();

        // Horizontal renders as <hr>
        Assert.Contains("flare-divider", cut.Find("hr").ClassName ?? "");
    }

    [Fact]
    public void Default_RendersHrElement()
    {
        var cut = Render<FlareDivider>();

        Assert.NotEmpty(cut.FindAll("hr"));
    }

    [Fact]
    public void Vertical_True_RendersDiv_NotHr()
    {
        var cut = Render<FlareDivider>(p => p
            .Add(x => x.Vertical, true));

        Assert.Empty(cut.FindAll("hr"));
        Assert.NotEmpty(cut.FindAll("div.flare-divider"));
    }

    [Fact]
    public void Vertical_True_AddsVerticalClass()
    {
        var cut = Render<FlareDivider>(p => p
            .Add(x => x.Vertical, true));

        Assert.Contains("flare-divider--vertical", cut.Find("div").ClassName ?? "");
    }

    [Fact]
    public void Vertical_False_Default_NoVerticalClass()
    {
        var cut = Render<FlareDivider>(p => p
            .Add(x => x.Vertical, false));

        Assert.DoesNotContain("flare-divider--vertical", cut.Find("hr").ClassName ?? "");
    }

    [Fact]
    public void AriaHidden_IsSet_OnHorizontalDivider()
    {
        var cut = Render<FlareDivider>();

        Assert.Equal("true", cut.Find("hr").GetAttribute("aria-hidden"));
    }
}

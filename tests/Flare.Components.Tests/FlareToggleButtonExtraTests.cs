// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

public class FlareToggleButtonExtraTests : FlareTestContext
{
    [Fact]
    public void SizeSmall_HasSmallClass()
    {
        var cut = Render<FlareToggleButton>(p => p
            .Add(x => x.Size, ButtonSize.Sm));

        Assert.Contains("flare-toggle-btn--sm", cut.Find("button").ClassName);
    }

    [Fact]
    public void SizeLarge_HasLargeClass()
    {
        var cut = Render<FlareToggleButton>(p => p
            .Add(x => x.Size, ButtonSize.Lg));

        Assert.Contains("flare-toggle-btn--lg", cut.Find("button").ClassName);
    }
}

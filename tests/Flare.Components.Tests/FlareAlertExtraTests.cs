// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

public class FlareAlertExtraTests : FlareTestContext
{
    [Fact]
    public void Dismissible_ShowsCloseButton()
    {
        var cut = RenderComponent<FlareAlert>(p => p
            .Add(x => x.Dismissible, true));

        Assert.NotEmpty(cut.FindAll(".flare-alert__close"));
    }

    [Fact]
    public void ShowCloseButton_ShowsCloseButton()
    {
        var cut = RenderComponent<FlareAlert>(p => p
            .Add(x => x.ShowCloseButton, true));

        Assert.NotEmpty(cut.FindAll(".flare-alert__close"));
    }
}

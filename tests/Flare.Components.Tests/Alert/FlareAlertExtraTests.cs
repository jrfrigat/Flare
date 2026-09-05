namespace Flare.Components.Tests;

public class FlareAlertExtraTests : FlareTestContext
{
    [Fact]
    public void Dismissible_ShowsCloseButton()
    {
        var cut = Render<FlareAlert>(p => p
            .Add(x => x.Dismissible, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Alert.Close}"));
    }

    [Fact]
    public void ShowCloseButton_ShowsCloseButton()
    {
        var cut = Render<FlareAlert>(p => p
            .Add(x => x.ShowCloseButton, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Alert.Close}"));
    }
}

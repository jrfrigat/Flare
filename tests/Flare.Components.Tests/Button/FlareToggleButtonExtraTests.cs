namespace Flare.Components.Tests;

public class FlareToggleButtonExtraTests : FlareTestContext
{
    [Fact]
    public void SizeSmall_HasSmallClass()
    {
        var cut = Render<FlareToggleButton>(p => p
            .Add(x => x.Size, ButtonSize.Sm));

        Assert.Contains(Css.Classes.Button.SizeSm, cut.Find("button").ClassName);
    }

    [Fact]
    public void SizeLarge_HasLargeClass()
    {
        var cut = Render<FlareToggleButton>(p => p
            .Add(x => x.Size, ButtonSize.Lg));

        Assert.Contains(Css.Classes.Button.SizeLg, cut.Find("button").ClassName);
    }
}

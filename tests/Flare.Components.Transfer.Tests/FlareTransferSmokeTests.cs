namespace Flare.Components.Tests;

/// <summary>
/// The package shipped with no test at all, so nothing said whether it still rendered. The floor: both
/// panels are drawn, and every item on the source side gets a row.
/// </summary>
public class FlareTransferSmokeTests : FlareTestContext
{
    private static readonly string[] Items = ["alpha", "beta", "gamma"];

    [Fact]
    public void RendersBothPanelsAndOneRowPerItem()
    {
        var cut = Render<FlareTransfer<string>>(p => p.Add(x => x.SourceItems, Items));

        Assert.NotEmpty(cut.FindAll(".flare-transfer"));
        Assert.Equal(2, cut.FindAll(".flare-transfer__panel").Count);
        Assert.Equal(Items.Length, cut.FindAll($".{Css.Classes.Transfer.Item}").Count);
    }

    [Fact]
    public void WithoutItems_StillRendersItsPanels()
    {
        var cut = Render<FlareTransfer<string>>(p => p.Add(x => x.SourceItems, []));

        Assert.Equal(2, cut.FindAll(".flare-transfer__panel").Count);
        Assert.Empty(cut.FindAll($".{Css.Classes.Transfer.Item}"));
    }
}

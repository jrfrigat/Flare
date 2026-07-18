namespace Flare.Components.Tests;

// The add-on Fluent pack renders inline SVG with a size-grid viewBox, subclasses the core FlareIcon from a
// separate assembly, and drops into any FlareIcon-typed slot.
public class FlareFluentUIIconTests : FlareTestContext
{
    [Fact]
    public void RendersSvgPath_WithGridViewBox()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, new FlareFluentUIIcon { Data = "M3 6h14v2H3z", GridSize = FluentUIIconSize.Size20 }));

        var svg = cut.Find("svg");
        Assert.Equal("0 0 20 20", svg.GetAttribute("viewBox"));
        Assert.Equal("M3 6h14v2H3z", cut.Find("path").GetAttribute("d"));
    }

    [Fact]
    public void DefaultGrid_Is24()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, new FlareFluentUIIcon { Data = "M0 0h24v24H0z", Filled = true }));

        Assert.Equal("0 0 24 24", cut.Find("svg").GetAttribute("viewBox"));
    }

    [Fact]
    public void FlowsIntoFlareIconButton_AsFlareIcon()
    {
        var cut = Render<FlareIconButton>(p => p
            .Add(x => x.Icon, new FlareFluentUIIcon { Data = "M3 6h14v2H3z", Name = "Home" })
            .Add(x => x.AriaLabel, "Home"));

        Assert.NotEmpty(cut.FindAll("svg path"));
    }

    [Fact]
    public void CuratedCatalog_RendersRealFluentSvg()
    {
        // Generated FluentUIIcons catalog carries real Fluent UI System Icons SVG (Size24 -> 24x24 viewBox).
        var cut = Render<FlareIconView>(p => p.Add(x => x.Value, FluentUIIcons.Regular.Home));

        var svg = cut.Find("svg");
        Assert.Equal("0 0 24 24", svg.GetAttribute("viewBox"));
        Assert.NotEmpty(cut.FindAll("svg path"));
        Assert.False(FluentUIIcons.Regular.Home.Filled);
        Assert.True(FluentUIIcons.Filled.Home.Filled);
    }
}

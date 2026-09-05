namespace Flare.Components.Tests;

// The Fluent UI System Icons SVG catalog ships as plain FlareSvgIcon values (Size 24 -> a 24x24 viewBox),
// consistent with the Material .Svg icon packages - there is no Fluent-specific icon type.
public class FluentUIIconsTests : FlareTestContext
{
    [Fact]
    public void Catalog_RendersRealFluentSvg_On24Grid()
    {
        var cut = Render<FlareIconView>(p => p.Add(x => x.Value, FluentUIIcons.Regular.Home));

        var svg = cut.Find("svg");
        Assert.Equal("0 0 24 24", svg.GetAttribute("viewBox"));
        Assert.NotEmpty(cut.FindAll("svg path"));
    }

    [Fact]
    public void RegularAndFilled_DifferByArtwork()
    {
        Assert.NotEqual(FluentUIIcons.Regular.Home.Data, FluentUIIcons.Filled.Home.Data);
    }

    [Fact]
    public void FlowsIntoFlareIconButton_AsFlareIcon()
    {
        var cut = Render<FlareIconButton>(p => p
            .Add(x => x.Icon, FluentUIIcons.Regular.Settings)
            .Add(x => x.AriaLabel, "Settings"));

        Assert.NotEmpty(cut.FindAll("svg path"));
    }
}

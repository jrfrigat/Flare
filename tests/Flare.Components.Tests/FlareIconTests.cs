namespace Flare.Components.Tests;

// FlareIcon must treat Material Symbol NAMES as font glyphs even when they start with a
// path-command letter (share -> 's', menu -> 'm', close -> 'c', list -> 'l', home -> 'h'),
// and only emit an <svg><path> when Icon is real path data or markup.
public class FlareIconTests : FlareTestContext
{
    [Theory]
    [InlineData("share")]
    [InlineData("menu")]
    [InlineData("close")]
    [InlineData("list")]
    [InlineData("home")]
    [InlineData("search")]
    public void IconName_RendersGlyph_NotPath(string name)
    {
        var cut = Render<FlareIcon>(p => p.Add(x => x.Icon, name));

        Assert.Empty(cut.FindAll("path"));
        Assert.Empty(cut.FindAll("svg"));
        Assert.Contains(name, cut.Markup);
    }

    [Fact]
    public void Name_RendersGlyph()
    {
        var cut = Render<FlareIcon>(p => p.Add(x => x.Name, "share"));

        Assert.Empty(cut.FindAll("path"));
        Assert.Contains("share", cut.Markup);
    }

    [Theory]
    [InlineData("M3 18h18v-2H3v2z")]
    [InlineData("m0,0 l10,10 z")]
    [InlineData("M12 0 A 5 5 0 0 1 7 7 Z")]
    public void PathData_RendersSvgPath(string d)
    {
        var cut = Render<FlareIcon>(p => p.Add(x => x.Icon, d));

        Assert.NotEmpty(cut.FindAll("svg path"));
        Assert.Equal(d, cut.Find("path").GetAttribute("d"));
    }

    [Fact]
    public void SvgMarkup_RendersAsSvg()
    {
        var cut = Render<FlareIcon>(p => p
            .Add(x => x.Icon, "<circle cx=\"12\" cy=\"12\" r=\"6\" />"));

        Assert.NotEmpty(cut.FindAll("svg"));
    }
}

using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// FlareIconView must treat Material Symbol NAMES as font glyphs even when they start with a
// path-command letter (share -> 's', menu -> 'm', close -> 'c', list -> 'l', home -> 'h'),
// and only emit an <svg><path> when the value is real path data or markup. The value types
// (FlareMaterialIcon / FlareSvgIcon) render the same output standalone and drop into any
// FlareIcon-typed slot.
public class FlareIconTests : FlareTestContext
{
    [Theory]
    [InlineData("share")]
    [InlineData("menu")]
    [InlineData("close")]
    [InlineData("list")]
    [InlineData("home")]
    [InlineData("search")]
    public void IconShortcut_RendersGlyph_NotPath(string name)
    {
        var cut = Render<FlareIconView>(p => p.Add(x => x.Icon, name));

        Assert.Empty(cut.FindAll("path"));
        Assert.Empty(cut.FindAll("svg"));
        Assert.Contains(name, cut.Markup);
    }

    [Fact]
    public void NameShortcut_RendersGlyph()
    {
        var cut = Render<FlareIconView>(p => p.Add(x => x.Name, "share"));

        Assert.Empty(cut.FindAll("path"));
        Assert.Contains("material-symbols-rounded", cut.Markup);
        Assert.Contains("share", cut.Markup);
    }

    [Theory]
    [InlineData("M3 18h18v-2H3v2z")]
    [InlineData("m0,0 l10,10 z")]
    [InlineData("M12 0 A 5 5 0 0 1 7 7 Z")]
    public void PathDataShortcut_RendersSvgPath(string d)
    {
        var cut = Render<FlareIconView>(p => p.Add(x => x.Icon, d));

        Assert.NotEmpty(cut.FindAll("svg path"));
        Assert.Equal(d, cut.Find("path").GetAttribute("d"));
    }

    [Fact]
    public void SvgMarkupShortcut_RendersAsSvg()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Icon, "<circle cx=\"12\" cy=\"12\" r=\"6\" />"));

        Assert.NotEmpty(cut.FindAll("svg"));
    }

    // ---- Value types render standalone via the Value parameter ----------------------------------

    [Fact]
    public void MaterialIconValue_RendersGlyph_WithAxes()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, new FlareMaterialIcon { Name = "star", Fill = true }));

        Assert.Empty(cut.FindAll("svg"));
        Assert.Contains("star", cut.Markup);
        Assert.Contains("'FILL' 1", cut.Markup);
    }

    [Fact]
    public void MaterialIconValue_Rounded_ByDefault_NoAxes_WhenUnset()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, new FlareMaterialIcon { Name = "home" }));

        Assert.Contains("material-symbols-rounded", cut.Markup);
        // Baseline axes are inherited from icon.css, so no per-icon override is emitted.
        Assert.DoesNotContain("font-variation-settings", cut.Markup);
    }

    [Fact]
    public void SvgIconValue_RendersPath()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, new FlareSvgIcon { Data = "M3 18h18v-2H3v2z" }));

        Assert.NotEmpty(cut.FindAll("svg path"));
        Assert.Equal("M3 18h18v-2H3v2z", cut.Find("path").GetAttribute("d"));
    }

    // ---- The abstract base is polymorphic: a string flows in as a Material icon ------------------

    [Fact]
    public void StringImplicitlyConverts_ToMaterialIcon()
    {
        FlareIcon icon = "settings";

        var material = Assert.IsType<FlareMaterialIcon>(icon);
        Assert.Equal("settings", material.Name);
    }

    [Fact]
    public void ViewValue_OverridesDescriptorSize()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, new FlareSvgIcon { Data = "M3 18h18v-2H3v2z", Size = "1rem" })
            .Add(x => x.Size, "3rem"));

        Assert.Contains("font-size:3rem", cut.Find("svg").GetAttribute("style"));
    }
}

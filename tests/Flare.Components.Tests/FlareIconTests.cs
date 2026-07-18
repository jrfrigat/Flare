namespace Flare.Components.Tests;

// A bare name resolves to the dependency-free built-in SVG when the id is in FlareIcons (e.g. "home",
// "share"), and only falls back to a Material Symbols glyph for ids that are not built in (e.g. "list",
// "volume_up"). Real path data / markup always renders inline SVG. Names starting with a path-command
// letter (share -> 's', menu -> 'm', list -> 'l') must still be treated as names, not path data.
public class FlareIconTests : FlareTestContext
{
    [Theory]
    [InlineData("share")]
    [InlineData("menu")]
    [InlineData("close")]
    [InlineData("home")]
    [InlineData("search")]
    public void CataloguedNameShortcut_RendersBuiltInSvg(string name)
    {
        var cut = Render<FlareIconView>(p => p.Add(x => x.Icon, name));

        // Built-in -> inline SVG, not the Material font.
        Assert.NotEmpty(cut.FindAll("svg path"));
        Assert.DoesNotContain("material-symbols", cut.Markup);
        Assert.Equal(FlareIcons.Find(name)!.Data, cut.Find("path").GetAttribute("d"));
    }

    [Theory]
    [InlineData("list")]
    [InlineData("sync")]
    [InlineData("chat")]
    [InlineData("cloud")]
    [InlineData("history")]
    public void UncataloguedNameShortcut_RendersEmpty(string name)
    {
        var cut = Render<FlareIconView>(p => p.Add(x => x.Icon, name));

        // Not built in -> empty icon; core never falls back to a Material font.
        Assert.Empty(cut.FindAll("path"));
        Assert.DoesNotContain("material-symbols", cut.Markup);
        Assert.DoesNotContain(name, cut.Markup);
    }

    [Fact]
    public void NameShortcut_Catalogued_IsBuiltInSvg()
    {
        var cut = Render<FlareIconView>(p => p.Add(x => x.Name, "home"));

        Assert.NotEmpty(cut.FindAll("svg path"));
        Assert.DoesNotContain("material-symbols", cut.Markup);
    }

    [Fact]
    public void NameShortcut_Uncatalogued_IsEmpty()
    {
        var cut = Render<FlareIconView>(p => p.Add(x => x.Name, "volume_up"));

        Assert.DoesNotContain("material-symbols", cut.Markup);
        Assert.Empty(cut.FindAll("path"));
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

    // ---- The string -> FlareIcon conversion prefers the built-in SVG, falling back to Material -----

    [Fact]
    public void StringImplicitlyConverts_Catalogued_ToBuiltInSvg()
    {
        FlareIcon icon = "settings";

        Assert.IsType<FlareSvgIcon>(icon);
    }

    [Fact]
    public void StringImplicitlyConverts_Uncatalogued_ToEmpty()
    {
        FlareIcon icon = "volume_up";

        // No third-party fallback: an unknown id resolves to the empty built-in icon.
        Assert.Same(FlareIcons.Empty, icon);
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

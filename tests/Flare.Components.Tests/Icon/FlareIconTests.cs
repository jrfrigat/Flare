namespace Flare.Components.Tests;

// FlareIconView renders a typed FlareIcon Value - there is no name-string lookup (that is what keeps the
// SVG icon packages trimmable). Caller overrides (Size/Color/AriaLabel) apply on top of the descriptor.
public class FlareIconTests : FlareTestContext
{
    [Fact]
    public void BuiltInValue_RendersInlineSvg()
    {
        var cut = Render<FlareIconView>(p => p.Add(x => x.Value, FlareIcons.Home));

        Assert.NotEmpty(cut.FindAll("svg path"));
        Assert.DoesNotContain("material-symbols", cut.Markup);
        Assert.Equal(FlareIcons.Home.Data, cut.Find("path").GetAttribute("d"));
    }

    [Fact]
    public void SvgIconValue_RendersPath()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, new FlareSvgIcon { Data = "M3 18h18v-2H3v2z" }));

        Assert.NotEmpty(cut.FindAll("svg path"));
        Assert.Equal("M3 18h18v-2H3v2z", cut.Find("path").GetAttribute("d"));
    }

    [Fact]
    public void NullValue_RendersEmptySvg_NoPath()
    {
        var cut = Render<FlareIconView>(p => p.Add(x => x.Value, (FlareIcon?)null));

        Assert.NotEmpty(cut.FindAll("svg"));
        Assert.Empty(cut.FindAll("path"));
    }

    // ---- Add-on pack value types render standalone via the Value parameter -----------------------

    [Fact]
    public void MaterialDesign3IconValue_RendersGlyph_WithAxes()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, new FlareMaterialDesign3Icon { Name = "star", Fill = true }));

        Assert.Empty(cut.FindAll("svg"));
        Assert.Contains("star", cut.Markup);
        Assert.Contains("'FILL' 1", cut.Markup);
    }

    [Fact]
    public void MaterialDesign3IconValue_Rounded_ByDefault_NoAxes_WhenUnset()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, new FlareMaterialDesign3Icon { Name = "home" }));

        Assert.Contains("material-symbols-rounded", cut.Markup);
        // Baseline axes are inherited from icon.css, so no per-icon override is emitted.
        Assert.DoesNotContain("font-variation-settings", cut.Markup);
    }

    // ---- Caller overrides win over the descriptor's own values ------------------------------------

    [Fact]
    public void ViewValue_OverridesDescriptorSize()
    {
        var cut = Render<FlareIconView>(p => p
            .Add(x => x.Value, new FlareSvgIcon { Data = "M3 18h18v-2H3v2z", Size = "1rem" })
            .Add(x => x.Size, "3rem"));

        Assert.Contains("font-size:3rem", cut.Find("svg").GetAttribute("style"));
    }
}

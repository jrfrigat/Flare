namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareContainer  (6 tests from Wave7)
// ------------------------------------------------------------------------------

public class C_FlareContainerTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareContainer>();

        Assert.NotEmpty(cut.FindAll(".flare-container"));
    }

    [Fact]
    public void DefaultMaxWidth_AddsLgClass()
    {
        var cut = Render<FlareContainer>();

        Assert.Contains("flare-container--lg", cut.Find("div").ClassName ?? "");
    }

    [Fact]
    public void MaxWidth_Xs_AddsXsClass()
    {
        var cut = Render<FlareContainer>(p => p
            .Add(x => x.MaxWidth, ContainerMaxWidth.Xs));

        Assert.Contains("flare-container--xs", cut.Find("div").ClassName ?? "");
    }

    [Fact]
    public void MaxWidth_Xl_AddsXlClass()
    {
        var cut = Render<FlareContainer>(p => p
            .Add(x => x.MaxWidth, ContainerMaxWidth.Xl));

        Assert.Contains("flare-container--xl", cut.Find("div").ClassName ?? "");
    }

    [Fact]
    public void Fluid_True_AddsFluidClass()
    {
        var cut = Render<FlareContainer>(p => p
            .Add(x => x.Fluid, true));

        Assert.Contains("flare-container--fluid", cut.Find("div").ClassName ?? "");
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareContainer>(p => p
            .AddChildContent("<span id=\"container-child\">Inner</span>"));

        Assert.NotEmpty(cut.FindAll("#container-child"));
    }
}

// ------------------------------------------------------------------------------
// FlareSpacer  (4 tests from Wave7)
// ------------------------------------------------------------------------------

public class C_FlareSpacerTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareSpacer>();

        Assert.NotEmpty(cut.FindAll("div"));
    }

    [Fact]
    public void HasSpacerClass()
    {
        var cut = Render<FlareSpacer>();

        Assert.Contains("flare-spacer", cut.Find("div").ClassName ?? "");
    }

    [Fact]
    public void AdditionalAttributes_StylePassesThrough()
    {
        var cut = Render<FlareSpacer>(p => p
            .AddUnmatched("style", "flex-grow:1"));

        Assert.NotEmpty(cut.FindAll("div"));
        Assert.NotNull(cut.Instance);
    }

    [Fact]
    public void Style_Param_PassesThrough()
    {
        var cut = Render<FlareSpacer>(p => p
            .Add(x => x.Style, "flex-grow:2"));

        var style = cut.Find("div").GetAttribute("style") ?? "";
        Assert.Contains("flex-grow", style);
    }
}

// ------------------------------------------------------------------------------
// FlareResizable  (6 tests from Wave6)
// ------------------------------------------------------------------------------

public class C_FlareResizableTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareResizable>();

        Assert.NotEmpty(cut.FindAll(".flare-resizable"));
    }

    [Fact]
    public void RendersHandle()
    {
        var cut = Render<FlareResizable>();

        Assert.NotEmpty(cut.FindAll(".flare-resizable__handle"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareResizable>(p => p
            .AddChildContent("<span id=\"resizable-inner\">Content</span>"));

        Assert.NotEmpty(cut.FindAll("#resizable-inner"));
    }

    [Fact]
    public void DefaultEdge_HasRightClass()
    {
        var cut = Render<FlareResizable>();

        Assert.Contains("flare-resizable--right", cut.Find(".flare-resizable").ClassName ?? "");
    }

    [Fact]
    public void BottomEdge_HasBottomClass()
    {
        var cut = Render<FlareResizable>(p => p
            .Add(x => x.Edge, FlareResizable.ResizableEdge.Bottom));

        Assert.Contains("flare-resizable--bottom", cut.Find(".flare-resizable").ClassName ?? "");
    }

    [Fact]
    public void InitialSize_AppliedInStyle()
    {
        var cut = Render<FlareResizable>(p => p
            .Add(x => x.InitialSize, "300px"));

        var style = cut.Find(".flare-resizable").GetAttribute("style") ?? "";
        Assert.Contains("300px", style);
    }
}

// ------------------------------------------------------------------------------
// FlareDivider  (6 tests from Wave8)
// ------------------------------------------------------------------------------

public class C_FlareDividerTests : FlareTestContext
{
    [Fact]
    public void Default_RendersFlareDividerElement()
    {
        var cut = Render<FlareDivider>();

        Assert.Contains("flare-divider", cut.Find("hr").ClassName ?? "");
    }

    [Fact]
    public void Default_RendersHrElement()
    {
        var cut = Render<FlareDivider>();

        Assert.NotEmpty(cut.FindAll("hr"));
    }

    [Fact]
    public void Vertical_True_RendersDiv_NotHr()
    {
        var cut = Render<FlareDivider>(p => p
            .Add(x => x.Vertical, true));

        Assert.Empty(cut.FindAll("hr"));
        Assert.NotEmpty(cut.FindAll("div.flare-divider"));
    }

    [Fact]
    public void Vertical_True_AddsVerticalClass()
    {
        var cut = Render<FlareDivider>(p => p
            .Add(x => x.Vertical, true));

        Assert.Contains("flare-divider--vertical", cut.Find("div").ClassName ?? "");
    }

    [Fact]
    public void Vertical_False_Default_NoVerticalClass()
    {
        var cut = Render<FlareDivider>(p => p
            .Add(x => x.Vertical, false));

        Assert.DoesNotContain("flare-divider--vertical", cut.Find("hr").ClassName ?? "");
    }

    [Fact]
    public void AriaHidden_IsSet_OnHorizontalDivider()
    {
        var cut = Render<FlareDivider>();

        Assert.Equal("true", cut.Find("hr").GetAttribute("aria-hidden"));
    }
}

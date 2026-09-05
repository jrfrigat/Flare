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

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Container.Root}"));
    }

    [Fact]
    public void DefaultMaxWidth_AddsLgClass()
    {
        var cut = Render<FlareContainer>();

        Assert.Contains(Css.Classes.Container.Lg, cut.Find("div").ClassName ?? "");
    }

    [Fact]
    public void MaxWidth_Xs_AddsXsClass()
    {
        var cut = Render<FlareContainer>(p => p
            .Add(x => x.MaxWidth, ContainerMaxWidth.Xs));

        Assert.Contains(Css.Classes.Container.Xs, cut.Find("div").ClassName ?? "");
    }

    [Fact]
    public void MaxWidth_Xl_AddsXlClass()
    {
        var cut = Render<FlareContainer>(p => p
            .Add(x => x.MaxWidth, ContainerMaxWidth.Xl));

        Assert.Contains(Css.Classes.Container.Xl, cut.Find("div").ClassName ?? "");
    }

    [Fact]
    public void Fluid_True_AddsFluidClass()
    {
        var cut = Render<FlareContainer>(p => p
            .Add(x => x.Fluid, true));

        Assert.Contains(Css.Classes.Container.Fluid, cut.Find("div").ClassName ?? "");
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

        Assert.Contains(Css.Classes.Spacer.Root, cut.Find("div").ClassName ?? "");
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

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Resizable.Root}"));
    }

    [Fact]
    public void RendersHandle()
    {
        var cut = Render<FlareResizable>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Resizable.Handle}"));
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

        Assert.Contains(Css.Classes.Resizable.Right, cut.Find($".{Css.Classes.Resizable.Root}").ClassName ?? "");
    }

    [Fact]
    public void BottomEdge_HasBottomClass()
    {
        var cut = Render<FlareResizable>(p => p
            .Add(x => x.Edge, ResizableEdge.Bottom));

        Assert.Contains(Css.Classes.Resizable.Bottom, cut.Find($".{Css.Classes.Resizable.Root}").ClassName ?? "");
    }

    [Fact]
    public void InitialSize_AppliedInStyle()
    {
        var cut = Render<FlareResizable>(p => p
            .Add(x => x.InitialSize, "300px"));

        var style = cut.Find($".{Css.Classes.Resizable.Root}").GetAttribute("style") ?? "";
        Assert.Contains("300px", style);
    }
}

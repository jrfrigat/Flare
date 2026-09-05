namespace Flare.Components.Tests;

public class FlareContainerTests : FlareTestContext
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

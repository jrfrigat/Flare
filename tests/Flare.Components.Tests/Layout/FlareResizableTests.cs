namespace Flare.Components.Tests;

public class FlareResizableTests : FlareTestContext
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

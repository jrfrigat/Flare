using Flare.Components.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareResizableTests : FlareTestContext
{
    private readonly RecordingJsRuntime _runtime;

    public FlareResizableTests()
    {
        _runtime = new RecordingJsRuntime(JSInterop.JSRuntime);
        Services.AddScoped<IResizeJsService>(_ => new ResizeJsService(_runtime));
    }

    [Theory]
    [InlineData(ResizableEdge.Right, "vertical")]
    [InlineData(ResizableEdge.Left, "vertical")]
    [InlineData(ResizableEdge.Top, "horizontal")]
    [InlineData(ResizableEdge.Bottom, "horizontal")]
    public void ProvidesNamedSeparatorAndIndependentStepButtons(ResizableEdge edge, string orientation)
    {
        var cut = Render<FlareResizable>(p => p.Add(x => x.Edge, edge));
        var handle = cut.Find("[role=separator]");
        Assert.Equal("0", handle.GetAttribute("tabindex"));
        Assert.Equal(orientation, handle.GetAttribute("aria-orientation"));
        Assert.False(string.IsNullOrWhiteSpace(handle.GetAttribute("aria-label")));
        Assert.Empty(handle.QuerySelectorAll("button"));
        var buttons = cut.FindAll("button[data-flare-resize-step]");
        Assert.Equal(2, buttons.Count);
        Assert.All(buttons, button =>
        {
            Assert.Equal("button", button.GetAttribute("type"));
            Assert.False(string.IsNullOrWhiteSpace(button.GetAttribute("aria-label")));
        });
    }

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

    [Fact]
    public void ChangingEdgeOrLimits_ReRegistersTheDragWithoutStackingCallbacks()
    {
        var cut = Render<FlareResizable>(p => p
            .Add(x => x.Edge, ResizableEdge.Right)
            .Add(x => x.MinSize, "120px")
            .Add(x => x.MaxSize, "480px"));
        cut.WaitForAssertion(() => Assert.Contains(_runtime.Invocations, i => i.Id == "registerResizeHandle"));

        cut.Render(p => p
            .Add(x => x.Edge, ResizableEdge.Bottom)
            .Add(x => x.MinSize, "80px")
            .Add(x => x.MaxSize, "300px"));

        cut.WaitForAssertion(() =>
        {
            var last = _runtime.Invocations.Last(i => i.Id == "registerResizeHandle");
            Assert.Equal("bottom", last.Args![2]);
            Assert.Equal("80px", last.Args[3]);
            Assert.Equal("300px", last.Args[4]);
        });

        // A render with unchanged configuration does not add another registration.
        var count = _runtime.Invocations.Count(i => i.Id == "registerResizeHandle");
        cut.Render(p => p
            .Add(x => x.Edge, ResizableEdge.Bottom)
            .Add(x => x.MinSize, "80px")
            .Add(x => x.MaxSize, "300px"));
        cut.WaitForAssertion(() =>
            Assert.Equal(count, _runtime.Invocations.Count(i => i.Id == "registerResizeHandle")));
    }
}

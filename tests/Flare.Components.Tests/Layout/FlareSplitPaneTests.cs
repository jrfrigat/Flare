using System.Diagnostics.CodeAnalysis;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// The split pane owns two panes, the persisted fraction and the resize handle. The handle itself is
/// FlareSplitter's; these tests cover the structure, the fraction contract and the hand-off from the
/// splitter's pixel report to the measured share.
/// </summary>
public class FlareSplitPaneTests : FlareTestContext
{
    private const string FirstSizeVariable = "--_flare-splitpane-first";
    private readonly StubSplitterJs _splitter = new();

    public FlareSplitPaneTests()
    {
        // A later registration wins, so the component's measure call lands in the stub.
        Services.AddScoped<ISplitterJsService>(_ => _splitter);
    }

    private static IRenderedComponent<FlareSplitPane> RenderPane(
        FlareSplitPaneTests owner,
        double fraction = 0.5,
        SplitPaneOrientation orientation = SplitPaneOrientation.Horizontal,
        EventCallback<double>? changed = null)
        => owner.Render<FlareSplitPane>(p =>
        {
            p.Add(x => x.Fraction, fraction);
            p.Add(x => x.Orientation, orientation);
            if (changed is { } callback) p.Add(x => x.FractionChanged, callback);
            p.Add(x => x.FirstPaneContent, (RenderFragment)(b => b.AddMarkupContent(0, "<p id='first'>First</p>")));
            p.Add(x => x.SecondPaneContent, (RenderFragment)(b => b.AddMarkupContent(0, "<p id='second'>Second</p>")));
        });

    [Fact]
    public void RendersBothPanesAndTheSharedHandle()
    {
        var cut = RenderPane(this);

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.SplitPane.First} #first"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.SplitPane.Second} #second"));
        Assert.NotEmpty(cut.FindAll("[role='separator']"));
        Assert.Contains(Css.Classes.SplitPane.Horizontal, cut.Find($".{Css.Classes.SplitPane.Root}").ClassName);
    }

    [Fact]
    public void VerticalOrientation_StacksThePanesAndFlipsTheHandleAxis()
    {
        var cut = RenderPane(this, orientation: SplitPaneOrientation.Vertical);

        Assert.Contains(Css.Classes.SplitPane.Vertical, cut.Find($".{Css.Classes.SplitPane.Root}").ClassName);
        Assert.Equal("vertical", cut.Find("[role='separator']").GetAttribute("data-flare-splitter-orientation"));
    }

    [Fact]
    public void Fraction_IsPublishedAsTheFirstPaneLength()
    {
        var cut = RenderPane(this, fraction: 0.25);

        Assert.Contains($"{FirstSizeVariable}:25%", cut.Find($".{Css.Classes.SplitPane.Root}").GetAttribute("style"));
    }

    [Theory]
    [InlineData(2.0, "100%")]
    [InlineData(-1.0, "0%")]
    public void Fraction_IsClamped(double fraction, string expected)
    {
        var cut = RenderPane(this, fraction: fraction);

        Assert.Contains($"{FirstSizeVariable}:{expected}", cut.Find($".{Css.Classes.SplitPane.Root}").GetAttribute("style"));
    }

    // The splitter reports pixels; the pane turns them into the share of the container, which is what
    // a host persists and restores when the container changes size.
    [Fact]
    public async Task SplitterReport_BecomesTheMeasuredFraction()
    {
        _splitter.Fraction = 0.7;
        var cut = RenderPane(this);

        await cut.InvokeAsync(() => cut.FindComponent<FlareSplitter>().Instance.OnSplitChanged("300px"));

        Assert.Contains($"{FirstSizeVariable}:70%", cut.Find($".{Css.Classes.SplitPane.Root}").GetAttribute("style"));
    }

    [Fact]
    public async Task SplitterReport_RaisesFractionChanged()
    {
        _splitter.Fraction = 0.7;
        double? raised = null;
        var cut = RenderPane(this, changed: EventCallback.Factory.Create<double>(this, value => raised = value));

        await cut.InvokeAsync(() => cut.FindComponent<FlareSplitter>().Instance.OnSplitChanged("300px"));

        Assert.Equal(0.7, raised);
    }

    [Fact]
    public async Task ControlledFraction_RevertsWhenTheParentDeclinesTheChange()
    {
        _splitter.Fraction = 0.7;
        var raised = 0;
        var changed = EventCallback.Factory.Create<double>(this, _ => raised++);
        var cut = RenderPane(this, fraction: 0.5, changed: changed);

        await cut.InvokeAsync(() => cut.FindComponent<FlareSplitter>().Instance.OnSplitChanged("300px"));
        Assert.Equal(1, raised);

        // The parent re-renders without moving Fraction: the pane goes back to what the parent says.
        cut.Render(p => p
            .Add(x => x.Fraction, 0.5)
            .Add(x => x.FractionChanged, changed));
        Assert.Contains($"{FirstSizeVariable}:50%", cut.Find($".{Css.Classes.SplitPane.Root}").GetAttribute("style"));
    }

    [Fact]
    public void ControlledFraction_AdoptsAnExternalChange()
    {
        var changed = EventCallback.Factory.Create<double>(this, _ => { });
        var cut = RenderPane(this, fraction: 0.5, changed: changed);

        cut.Render(p => p
            .Add(x => x.Fraction, 0.3)
            .Add(x => x.FractionChanged, changed));

        Assert.Contains($"{FirstSizeVariable}:30%", cut.Find($".{Css.Classes.SplitPane.Root}").GetAttribute("style"));
    }

    private sealed class StubSplitterJs : ISplitterJsService
    {
        public double Fraction { get; set; } = 0.5;

        public ValueTask RegisterAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(
            ElementReference gutter, string axis, string? minSize, string? maxSize, DotNetObjectReference<T> dotNetRef)
            where T : class => ValueTask.CompletedTask;

        public ValueTask NudgeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(
            ElementReference gutter, string axis, int deltaPx, string keyAxis, string? minSize, string? maxSize,
            DotNetObjectReference<T> dotNetRef) where T : class => ValueTask.CompletedTask;

        public ValueTask RemoveAsync(ElementReference gutter) => ValueTask.CompletedTask;

        public ValueTask<double> MeasureFractionAsync(ElementReference scope, ElementReference target, string axis) =>
            ValueTask.FromResult(Fraction);

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// A debounced keystroke is a write that has not happened yet. A commit, an external value change, a
/// change that makes the control non-editable or a configuration change means that text no longer
/// belongs to the field, so it must never land later - and a commit must not publish the same text a
/// second time.
/// </summary>
public class FlareTextInputDebounceTests : FlareTestContext
{
    private static CancellationToken Ct => Xunit.TestContext.Current.CancellationToken;

    private IRenderedComponent<FlareTextInput> RenderImmediate(
        List<string?> values, string? value = null, int debounce = 150, bool disabled = false, bool readOnly = false)
        => Render<FlareTextInput>(p => p
            .Add(x => x.Immediate, true)
            .Add(x => x.DebounceInterval, debounce)
            .Add(x => x.Value, value)
            .Add(x => x.Disabled, disabled)
            .Add(x => x.ReadOnly, readOnly)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => values.Add(v))));

    [Fact]
    public async Task Change_CancelsThePendingWriteAndPublishesOnce()
    {
        var values = new List<string?>();
        var cut = RenderImmediate(values);
        var input = cut.Find("input");

        input.Input("A");
        input.Change("A");
        await Task.Delay(300, Ct);

        Assert.Equal(new[] { "A" }, values);
    }

    [Fact]
    public async Task RapidTyping_PublishesOnlyTheLastKeystroke()
    {
        var values = new List<string?>();
        var cut = RenderImmediate(values);
        var input = cut.Find("input");

        input.Input("A");
        input.Input("AB");
        input.Input("ABC");
        await Task.Delay(300, Ct);

        Assert.Equal(new[] { "ABC" }, values);
    }

    [Fact]
    public async Task ExternalValueChange_CancelsThePendingWrite()
    {
        var values = new List<string?>();
        var cut = RenderImmediate(values, value: "start");
        cut.Find("input").Input("A");

        cut.Render(p => p
            .Add(x => x.Immediate, true)
            .Add(x => x.DebounceInterval, 150)
            .Add(x => x.Value, "reset")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => values.Add(v))));
        await Task.Delay(300, Ct);

        Assert.Empty(values);
    }

    [Fact]
    public async Task Disabled_CancelsThePendingWrite()
    {
        var values = new List<string?>();
        var cut = RenderImmediate(values, value: "start");
        cut.Find("input").Input("A");

        cut.Render(p => p
            .Add(x => x.Immediate, true)
            .Add(x => x.DebounceInterval, 150)
            .Add(x => x.Value, "start")
            .Add(x => x.Disabled, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => values.Add(v))));
        await Task.Delay(300, Ct);

        Assert.Empty(values);
    }

    [Fact]
    public async Task ReadOnly_CancelsThePendingWrite()
    {
        var values = new List<string?>();
        var cut = RenderImmediate(values, value: "start");
        cut.Find("input").Input("A");

        cut.Render(p => p
            .Add(x => x.Immediate, true)
            .Add(x => x.DebounceInterval, 150)
            .Add(x => x.Value, "start")
            .Add(x => x.ReadOnly, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => values.Add(v))));
        await Task.Delay(300, Ct);

        Assert.Empty(values);
    }

    [Fact]
    public async Task ADebounceIntervalChange_CancelsThePendingWrite()
    {
        var values = new List<string?>();
        var cut = RenderImmediate(values, value: "start");
        cut.Find("input").Input("A");

        cut.Render(p => p
            .Add(x => x.Immediate, true)
            .Add(x => x.DebounceInterval, 10)
            .Add(x => x.Value, "start")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => values.Add(v))));
        await Task.Delay(300, Ct);

        Assert.Empty(values);
    }

    [Fact]
    public async Task DisposingTheControl_CancelsThePendingWrite()
    {
        var values = new List<string?>();
        var cut = RenderImmediate(values);
        cut.Find("input").Input("A");

        await DisposeComponentsAsync();
        await Task.Delay(300, Ct);

        Assert.Empty(values);
    }
}

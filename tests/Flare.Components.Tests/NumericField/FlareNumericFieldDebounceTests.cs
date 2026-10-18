using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// A pending debounced unclamped commit must not land after a clamped change, a clear, a step or an
/// external value change: the later event owns the value.
/// </summary>
public class FlareNumericFieldDebounceTests : FlareTestContext
{
    [Fact]
    public async Task Change_CancelsThePendingDebouncedCommit()
    {
        var values = new List<double>();
        var cut = Render<FlareNumericField<double>>(p => p
            .Add(x => x.Immediate, true)
            .Add(x => x.DebounceInterval, 150)
            .Add(x => x.Max, 10.0)
            .Add(x => x.Value, 0.0)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, values.Add)));
        var input = cut.Find("input");

        input.Input("99");
        input.Change("99");
        await Task.Delay(300, Xunit.TestContext.Current.CancellationToken);

        Assert.Equal(new[] { 10.0 }, values);
    }

    [Fact]
    public async Task Clear_CancelsThePendingDebouncedCommit()
    {
        var values = new List<double>();
        var cut = Render<FlareNumericField<double>>(p => p
            .Add(x => x.Immediate, true)
            .Add(x => x.DebounceInterval, 150)
            .Add(x => x.Clearable, true)
            .Add(x => x.Value, 5.0)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, values.Add)));
        var input = cut.Find("input");

        input.Input("99");
        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Input.Clear}").Click());
        await Task.Delay(300, Xunit.TestContext.Current.CancellationToken);

        Assert.DoesNotContain(99.0, values);
        Assert.Contains(0.0, values);
    }

    [Fact]
    public async Task Step_CancelsThePendingDebouncedCommit()
    {
        var values = new List<double>();
        var cut = Render<FlareNumericField<double>>(p => p
            .Add(x => x.Immediate, true)
            .Add(x => x.DebounceInterval, 150)
            .Add(x => x.Step, 1.0)
            .Add(x => x.ShowStepper, true)
            .Add(x => x.Value, 5.0)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, values.Add)));
        var input = cut.Find("input");

        input.Input("99");
        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Numericfield.StepUp}").Click());
        await Task.Delay(300, Xunit.TestContext.Current.CancellationToken);

        Assert.DoesNotContain(99.0, values);
        Assert.Contains(6.0, values);
    }

    [Fact]
    public async Task ExternalValueChange_CancelsThePendingDebouncedCommit()
    {
        var values = new List<double>();
        var cut = Render<FlareNumericField<double>>(p => p
            .Add(x => x.Immediate, true)
            .Add(x => x.DebounceInterval, 150)
            .Add(x => x.Value, 0.0)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, values.Add)));
        cut.Find("input").Input("99");

        cut.Render(p => p
            .Add(x => x.Immediate, true)
            .Add(x => x.DebounceInterval, 150)
            .Add(x => x.Value, 7.0)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, values.Add)));
        await Task.Delay(300, Xunit.TestContext.Current.CancellationToken);

        Assert.DoesNotContain(99.0, values);
    }

    [Fact]
    public async Task Disabled_CancelsThePendingDebouncedCommit()
    {
        var values = new List<double>();
        var cut = Render<FlareNumericField<double>>(p => p
            .Add(x => x.Immediate, true)
            .Add(x => x.DebounceInterval, 150)
            .Add(x => x.Value, 0.0)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, values.Add)));
        cut.Find("input").Input("99");

        cut.Render(p => p
            .Add(x => x.Immediate, true)
            .Add(x => x.DebounceInterval, 150)
            .Add(x => x.Disabled, true)
            .Add(x => x.Value, 0.0)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<double>(this, values.Add)));
        await Task.Delay(300, Xunit.TestContext.Current.CancellationToken);

        Assert.DoesNotContain(99.0, values);
    }
}

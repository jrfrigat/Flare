using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// decimal and integral values keep every digit through commit and step. A silent zero on overflow is
/// worse than no change: the component keeps the previous value instead.
/// </summary>
public class FlareNumericFieldPrecisionTests : FlareTestContext
{
    [Fact]
    public void Change_KeepsLongPrecisionAboveDouble()
    {
        long value = 0;
        var cut = Render<FlareNumericField<long>>(p => p
            .Add(x => x.Value, 9007199254740993L)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<long>(this, v => value = v)));

        cut.Find("input").Change("9007199254740993");

        Assert.Equal(9007199254740993L, value);
    }

    [Fact]
    public void Change_KeepsDecimalPrecision()
    {
        const decimal expected = 0.1234567890123456789012345678m;
        decimal value = 0;
        var cut = Render<FlareNumericField<decimal>>(p => p
            .Add(x => x.Value, expected)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<decimal>(this, v => value = v)));

        cut.Find("input").Change("0.1234567890123456789012345678");

        Assert.Equal(expected, value);
    }

    [Fact]
    public async Task StepUp_KeepsLongPrecisionAboveDouble()
    {
        long value = 0;
        var cut = Render<FlareNumericField<long>>(p => p
            .Add(x => x.Value, 9007199254740993L)
            .Add(x => x.Step, 1.0)
            .Add(x => x.ShowStepper, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<long>(this, v => value = v)));

        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Numericfield.StepUp}").Click());

        Assert.Equal(9007199254740994L, value);
    }

    [Fact]
    public void Change_KeepsUlongMax()
    {
        ulong value = 0;
        var cut = Render<FlareNumericField<ulong>>(p => p
            .Add(x => x.Value, ulong.MaxValue)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<ulong>(this, v => value = v)));

        cut.Find("input").Change("18446744073709551615");

        Assert.Equal(ulong.MaxValue, value);
    }

    [Fact]
    public async Task StepUp_AtUlongMax_DoesNotWrapOrEmitZero()
    {
        var changed = false;
        var cut = Render<FlareNumericField<ulong>>(p => p
            .Add(x => x.Value, ulong.MaxValue)
            .Add(x => x.Step, 1.0)
            .Add(x => x.ShowStepper, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<ulong>(this, _ => changed = true)));

        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Numericfield.StepUp}").Click());

        Assert.False(changed);
    }

    [Fact]
    public void Change_OverflowWithoutBounds_KeepsPreviousValue()
    {
        var changed = false;
        var cut = Render<FlareNumericField<int>>(p => p
            .Add(x => x.Value, 7)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int>(this, _ => changed = true)));

        cut.Find("input").Change("999999999999999999999");

        Assert.False(changed);
    }

    [Fact]
    public void Change_EmptyOnNullable_EmitsNull()
    {
        int? value = 5;
        var cut = Render<FlareNumericField<int?>>(p => p
            .Add(x => x.Value, 5)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int?>(this, v => value = v)));

        cut.Find("input").Change("");

        Assert.Null(value);
    }
}

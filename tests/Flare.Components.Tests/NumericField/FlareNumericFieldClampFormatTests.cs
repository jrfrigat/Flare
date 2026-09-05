using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// C1 enterprise gaps closed: numeric clamp/format, slider marks, tag suggestions.
public class FlareNumericFieldClampFormatTests : FlareTestContext
{
    [Fact]
    public void Change_AboveMax_ClampsToMax()
    {
        var value = 0;
        var cut = Render<FlareNumericField<int>>(p => p
            .Add(x => x.Min, 0.0)
            .Add(x => x.Max, 10.0)
            .Add(x => x.Value, 0)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int>(this, v => value = v)));

        cut.Find("input").Change("999");
        Assert.Equal(10, value);
    }

    [Fact]
    public void Change_BelowMin_ClampsToMin()
    {
        var value = 0;
        var cut = Render<FlareNumericField<int>>(p => p
            .Add(x => x.Min, 5.0)
            .Add(x => x.Max, 10.0)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int>(this, v => value = v)));

        cut.Find("input").Change("-3");
        Assert.Equal(5, value);
    }

    [Fact]
    public void Format_SwitchesToTextModeInput()
    {
        var cut = Render<FlareNumericField<decimal>>(p => p
            .Add(x => x.Format, "N0")
            .Add(x => x.Value, 1234567m));

        var input = cut.Find("input");
        Assert.Equal("text", input.GetAttribute("type"));
        // Blurred display shows group separators (invariant culture uses comma).
        Assert.Contains(",", input.GetAttribute("value")!);
    }

    [Fact]
    public void NoFormat_StaysNumberInput()
    {
        var cut = Render<FlareNumericField<int>>(p => p.Add(x => x.Value, 42));
        Assert.Equal("number", cut.Find("input").GetAttribute("type"));
    }
}

using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class NumericFieldKeyboardTests : FlareTestContext
{
    [Fact]
    public void NumberInput_LeavesArrowSteppingToTheNativeControl()
    {
        var changes = 0;
        var keys = 0;
        var cut = Render<FlareNumericField<int>>(p => p
            .Add(x => x.Value, 42)
            .Add(x => x.Min, 0.0)
            .Add(x => x.Max, 100.0)
            .Add(x => x.Step, 1.0)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int>(this, (int _) => changes++))
            .Add(x => x.OnKeyDown, EventCallback.Factory.Create<KeyboardEventArgs>(this, (KeyboardEventArgs _) => keys++)));

        cut.Find("input").KeyDown("ArrowUp");

        Assert.Equal(0, changes);
        Assert.Equal(1, keys);
    }

    [Fact]
    public void TextMode_ArrowKeyStepsOnce()
    {
        var values = new List<int>();
        var keys = 0;
        var cut = Render<FlareNumericField<int>>(p => p
            .Add(x => x.Format, "N0")
            .Add(x => x.Culture, CultureInfo.GetCultureInfo("en-US"))
            .Add(x => x.Value, 42)
            .Add(x => x.Min, 0.0)
            .Add(x => x.Max, 100.0)
            .Add(x => x.Step, 1.0)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<int>(this, (int v) => values.Add(v)))
            .Add(x => x.OnKeyDown, EventCallback.Factory.Create<KeyboardEventArgs>(this, (KeyboardEventArgs _) => keys++)));

        cut.Find("input").KeyDown("ArrowUp");

        Assert.Equal([43], values);
        Assert.Equal(1, keys);
    }
}

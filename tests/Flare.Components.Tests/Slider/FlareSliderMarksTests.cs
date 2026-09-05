using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareSliderMarksTests : FlareTestContext
{
    [Fact]
    public void Marks_RenderLabeledMarks()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0)
            .Add(x => x.Max, 100)
            .Add(x => x.Value, 50)
            .Add(x => x.Marks, new Dictionary<double, string> { [0] = "Low", [50] = "Mid", [100] = "High" }));

        var marks = cut.FindAll($".{Css.Classes.Slider.Mark}");
        Assert.Equal(3, marks.Count);
        Assert.Contains(marks, m => m.TextContent == "Mid");
        Assert.Contains(Css.Classes.Slider.WithMarks, cut.Find($".{Css.Classes.Slider.Root}").ClassName);
    }

    [Fact]
    public void Marks_OutOfRange_AreIgnored()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0)
            .Add(x => x.Max, 100)
            .Add(x => x.Marks, new Dictionary<double, string> { [0] = "A", [200] = "B" }));

        Assert.Single(cut.FindAll($".{Css.Classes.Slider.Mark}"));
    }
}

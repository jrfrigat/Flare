using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareTimePickerTests : FlareTestContext
{
    [Fact]
    public void RendersRoot()
    {
        var cut = Render<FlareTimePicker>(p => p
            .Add(x => x.Value, new TimeOnly(14, 30))
            .Add(x => x.Label, "Time"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.TimePicker.Root}"));
        Assert.Contains("Time", cut.Markup);
    }
}

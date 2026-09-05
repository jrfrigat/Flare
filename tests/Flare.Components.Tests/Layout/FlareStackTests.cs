using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareStackTests : FlareTestContext
{
    [Fact]
    public void RendersRootAndChildContent()
    {
        var cut = Render<FlareStack>(p => p
            .AddChildContent("<span class=\"kid\">x</span>"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Stack.Root}"));
        Assert.NotEmpty(cut.FindAll(".kid"));
    }

    [Fact]
    public void Row_AddsRowModifier()
    {
        var cut = Render<FlareStack>(p => p.Add(x => x.Row, true));
        Assert.Contains(Css.Classes.Stack.Row, cut.Find($".{Css.Classes.Stack.Root}").ClassName);
    }
}

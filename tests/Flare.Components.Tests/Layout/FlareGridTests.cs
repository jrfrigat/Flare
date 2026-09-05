using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareGridTests : FlareTestContext
{
    [Fact]
    public void RendersRootAndChildContent()
    {
        var cut = Render<FlareGrid>(p => p
            .Add(x => x.Columns, 3)
            .AddChildContent("<div class=\"cell\">c</div>"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Grid.Root}"));
        Assert.NotEmpty(cut.FindAll(".cell"));
    }

    [Fact]
    public void MinColumnWidth_EmitsAutoFillTemplate()
    {
        var cut = Render<FlareGrid>(p => p
            .Add(x => x.MinColumnWidth, "15rem")
            .Add(x => x.Columns, 4));
        var style = cut.Find($".{Css.Classes.Grid.Root}").GetAttribute("style") ?? "";
        Assert.Contains("repeat(auto-fill,minmax(15rem,1fr))", style);
        // MinColumnWidth overrides the fixed Columns track set.
        Assert.DoesNotContain("repeat(4,1fr)", style);
    }

    [Fact]
    public void WithoutMinColumnWidth_EmitsFixedColumnTemplate()
    {
        var cut = Render<FlareGrid>(p => p.Add(x => x.Columns, 4));
        var style = cut.Find($".{Css.Classes.Grid.Root}").GetAttribute("style") ?? "";
        Assert.Contains("repeat(4,1fr)", style);
    }
}

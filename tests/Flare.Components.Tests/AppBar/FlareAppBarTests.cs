using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareAppBarTests : FlareTestContext
{
    [Fact]
    public void RendersTitle()
    {
        var cut = Render<FlareAppBar>(p => p.Add(x => x.Title, "My App"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.AppBar.Root}"));
        Assert.Contains("My App", cut.Markup);
    }

    [Fact]
    public void Sticky_AddsStickyModifier()
    {
        var cut = Render<FlareAppBar>(p => p
            .Add(x => x.Title, "T")
            .Add(x => x.Sticky, true));
        Assert.Contains(Css.Classes.AppBar.Sticky, cut.Find($".{Css.Classes.AppBar.Root}").ClassName);
    }
}

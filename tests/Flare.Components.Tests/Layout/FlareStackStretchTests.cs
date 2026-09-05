using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

// ------------------------------------------------------------------------------
// FlareStack StretchItems / StretchFirst
// ------------------------------------------------------------------------------
public class FlareStackStretchTests : FlareTestContext
{
    [Fact]
    public void StretchItems_AddsStretchClass()
    {
        var cut = Render<FlareStack>(p => p.Add(x => x.StretchItems, true));

        Assert.Contains(Css.Classes.Stack.Stretch, cut.Find($".{Css.Classes.Stack.Root}").ClassName);
    }

    [Fact]
    public void StretchFirst_AddsStretchFirstClass()
    {
        var cut = Render<FlareStack>(p => p.Add(x => x.StretchFirst, true));

        Assert.Contains(Css.Classes.Stack.StretchFirst, cut.Find($".{Css.Classes.Stack.Root}").ClassName);
    }

    [Fact]
    public void StretchItems_WinsOverStretchFirst()
    {
        var cut = Render<FlareStack>(p => p
            .Add(x => x.StretchItems, true)
            .Add(x => x.StretchFirst, true));

        var cls = cut.Find($".{Css.Classes.Stack.Root}").ClassName;
        Assert.Contains($"{Css.Classes.Stack.Stretch} ", cls + " ");
        Assert.DoesNotContain(Css.Classes.Stack.StretchFirst, cls);
    }
}

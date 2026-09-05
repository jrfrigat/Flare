using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareButtonShapeTests : FlareTestContext
{
    [Fact]
    public void Square_AddsShapeModifier()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Shape, ButtonShape.Square)
            .AddChildContent("Go"));
        Assert.Contains(Css.Classes.Button.Square, cut.Find("button").ClassName);
    }

    [Fact]
    public void Rounded_AddsRoundedModifier()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Shape, ButtonShape.Rounded)
            .AddChildContent("Go"));
        Assert.Contains(Css.Classes.Button.Rounded, cut.Find("button").ClassName);
    }

    [Fact]
    public void Circular_AddsCircularModifier()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Shape, ButtonShape.Circular)
            .AddChildContent("Go"));
        Assert.Contains(Css.Classes.Button.Circular, cut.Find("button").ClassName);
    }

    [Fact]
    public void Default_IsDefaultWithoutShapeModifier()
    {
        var cut = Render<FlareButton>(p => p.AddChildContent("Go"));
        var cls = cut.Find("button").ClassName;
        Assert.DoesNotContain(Css.Classes.Button.Square, cls);
        Assert.DoesNotContain(Css.Classes.Button.Rounded, cls);
        Assert.DoesNotContain(Css.Classes.Button.Circular, cls);
    }

    [Fact]
    public void ShapeMorph_IsNotAComponentConcern()
    {
        // The morph used to be a PressMorph parameter that stamped flare-btn--morph. It is now the
        // theme's --flare-btn-shape-morph-duration, which is why no class records it any more: a
        // theme could not deliver its own specified behaviour while every call site had to opt in.
        var cut = Render<FlareButton>(p => p.AddChildContent("Go"));

        Assert.DoesNotContain("morph", cut.Find("button").ClassName);
    }
}

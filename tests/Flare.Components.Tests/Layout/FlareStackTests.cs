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

    // A panel that carries role/tabindex needs a way to be focused programmatically - otherwise the only
    // way to place focus inside it is a raw element.
    [Fact]
    public async Task FocusAsync_MovesFocusToTheStackItself()
    {
        var cut = Render<FlareStack>(p => p.AddUnmatched("tabindex", "-1"));

        await cut.InvokeAsync(() => cut.Instance.FocusAsync().AsTask());

        Assert.Contains(JSInterop.Invocations, i => i.Identifier == "Blazor._internal.domWrapper.focus");
        Assert.Equal("-1", cut.Find($".{Css.Classes.Stack.Root}").GetAttribute("tabindex"));
    }

    [Fact]
    public void RootElement_ExposesTheRenderedElement()
    {
        var cut = Render<FlareStack>();

        Assert.False(string.IsNullOrEmpty(cut.Instance.RootElement.Id));
    }

    [Fact]
    public void Block_LaysChildrenOutInNormalFlow()
    {
        var cut = Render<FlareStack>(p => p
            .Add(x => x.Block, true)
            .AddChildContent("<button>one</button>"));

        Assert.Contains(Css.Classes.Stack.Block, cut.Find($".{Css.Classes.Stack.Root}").ClassName);
    }
}

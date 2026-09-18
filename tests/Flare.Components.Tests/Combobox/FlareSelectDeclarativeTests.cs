using Flare.Components.Combobox;
using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

// ------------------------------------------------------------------------------
// FlareSelect declarative <option> child content
// ------------------------------------------------------------------------------
public class FlareSelectDeclarativeTests : FlareTestContext
{
    private static RenderFragment Options => b =>
    {
        b.OpenElement(0, "option");
        b.AddAttribute(1, "value", "a");
        b.AddContent(2, "Apple");
        b.CloseElement();
        b.OpenElement(3, "option");
        b.AddAttribute(4, "value", "b");
        b.AddContent(5, "Banana");
        b.CloseElement();
    };

    [Fact]
    public void DeclarativeOptions_RenderInDropdown()
    {
        var cut = Render<FlareSelect<string>>(p => p.Add(x => x.ChildContent, Options));

        cut.Find($".{Css.Classes.Select.Control}").Click();

        Assert.Equal(2, cut.FindAll($".{Css.Classes.Select.Option}").Count);
    }

    [Fact]
    public void DeclarativeOptions_SelectedLabelShown()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Value, "b")
            .Add(x => x.ChildContent, Options));

        Assert.Contains("Banana", cut.Find($".{Css.Classes.Select.Value}").TextContent);
    }

    // Static <option> markup is compiled by Razor into a single Markup frame (raw HTML), not element
    // frames - bUnit's AddChildContent(string) reproduces that shape, which the parser must handle.
    [Fact]
    public void StaticOptionMarkup_RendersOptions()
    {
        var cut = Render<FlareSelect<string>>(p => p.AddChildContent(
            "<option value=\"a\">Apple</option><option value=\"b\">Banana</option><option value=\"c\">Cherry</option>"));

        cut.Find($".{Css.Classes.Select.Control}").Click();

        Assert.Equal(3, cut.FindAll($".{Css.Classes.Select.Option}").Count);
    }

    [Fact]
    public void StaticOptionMarkup_SelectedLabelShown()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Value, "b")
            .AddChildContent("<option value=\"a\">Apple</option><option value=\"b\">Banana</option>"));

        Assert.Contains("Banana", cut.Find($".{Css.Classes.Select.Value}").TextContent);
    }

    // A wrapper component that writes @ChildContent between the tags compiles to a fragment that is
    // never null, even when its own caller passed no children. Choosing the source on "is not null"
    // therefore threw the Items away and left the list empty, with nothing wrong at the call site.
    private static RenderFragment Empty => _ => { };

    [Fact]
    public void EmptyChildContent_FallsBackToItems()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, new[] { "a", "b", "c" })
            .Add(x => x.ChildContent, Empty));

        cut.Find($".{Css.Classes.Select.Control}").Click();

        Assert.Equal(3, cut.FindAll($".{Css.Classes.Select.Option}").Count);
    }

    [Fact]
    public void EmptyChildContent_WithNullOption_RendersItemsBesideTheNullRow()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, new[] { "a", "b", "c" })
            .Add(x => x.NullOption, "No value")
            .Add(x => x.ChildContent, Empty));

        cut.Find($".{Css.Classes.Select.Control}").Click();

        // The null row plus every item - the reported failure showed the null row alone.
        Assert.Equal(4, cut.FindAll($".{Css.Classes.Select.Option}").Count);
    }

    [Fact]
    public void ParseOrNull_NullContent_ReturnsNull()
        => Assert.Null(DeclaredOptions.ParseOrNull<string>(null));

    [Fact]
    public void ParseOrNull_EmptyFragment_ReturnsNull()
        => Assert.Null(DeclaredOptions.ParseOrNull<string>(Empty));

    [Fact]
    public void ParseOrNull_DeclaredOptions_ReturnsTheSet()
    {
        var set = DeclaredOptions.ParseOrNull<string>(Options);

        Assert.NotNull(set);
        Assert.Equal(new[] { "a", "b" }, set!.Values);
    }

    [Fact]
    public void Parse_EmptyFragment_ReportsNothingDeclared()
        => Assert.False(DeclaredOptions.Parse<string>(Empty).Any);

    [Fact]
    public void DeclaredOptions_StillWinOverItems()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, new[] { "x", "y", "z", "w" })
            .Add(x => x.ChildContent, Options));

        cut.Find($".{Css.Classes.Select.Control}").Click();

        Assert.Equal(2, cut.FindAll($".{Css.Classes.Select.Option}").Count);
    }
}

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

// A declared `<option disabled>` means what ItemDisabled means: visible, not selectable, skipped by the
// arrow keys. Without it an option that exists but is not yet offered had to leave the declarative API
// for Items plus three delegates - for one option.
public class FlareSelectDeclarativeDisabledTests : FlareTestContext
{
    // Element frames: how a dynamic or attribute-bound <option> compiles.
    private static RenderFragment Options => b =>
    {
        b.OpenElement(0, "option");
        b.AddAttribute(1, "value", "Shared");
        b.AddContent(2, "Shared working copy");
        b.CloseElement();
        b.OpenElement(3, "option");
        b.AddAttribute(4, "value", "Worktree");
        b.AddAttribute(5, "disabled", true);
        b.AddContent(6, "Separate worktree");
        b.CloseElement();
    };

    [Fact]
    public void Parse_ReadsDisabledFromElementFrames()
    {
        var set = DeclaredOptions.ParseOrNull<string>(Options);

        Assert.NotNull(set);
        Assert.False(set!.IsDisabled("Shared"));
        Assert.True(set.IsDisabled("Worktree"));
    }

    // Static markup compiles to one raw-HTML frame, where the attribute is bare and has no value at all.
    [Fact]
    public void Parse_ReadsBareDisabledFromRawMarkup()
    {
        var set = DeclaredOptions.ParseOrNull<string>(b => b.AddMarkupContent(0,
            "<option value=\"a\">Apple</option><option value=\"b\" disabled>Banana</option>"));

        Assert.NotNull(set);
        Assert.False(set!.IsDisabled("a"));
        Assert.True(set.IsDisabled("b"));
    }

    [Fact]
    public void Parse_HonoursAnExplicitFalse()
    {
        var set = DeclaredOptions.ParseOrNull<string>(b => b.AddMarkupContent(0,
            "<option value=\"a\" disabled=\"false\">Apple</option>"));

        Assert.NotNull(set);
        Assert.False(set!.IsDisabled("a"));
    }

    [Fact]
    public void Parse_TreatsAQuotedDisabledAsSet()
    {
        var set = DeclaredOptions.ParseOrNull<string>(b => b.AddMarkupContent(0,
            "<option value=\"a\" disabled=\"disabled\">Apple</option>"));

        Assert.True(set!.IsDisabled("a"));
    }

    [Fact]
    public void DisabledOption_RendersInTheListAndIsMarkedDisabled()
    {
        var cut = Render<FlareSelect<string>>(p => p.Add(x => x.ChildContent, Options));

        cut.Find($".{Css.Classes.Select.Control}").Click();

        var options = cut.FindAll($".{Css.Classes.Select.Option}");
        Assert.Equal(2, options.Count);                                  // still visible
        Assert.Equal("true", options[1].GetAttribute("aria-disabled"));
    }

    [Fact]
    public void DisabledOption_DoesNotCommitOnClick()
    {
        string? picked = null;
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.ChildContent, Options)
            .Add(x => x.ValueChanged, (string? v) => picked = v));

        cut.Find($".{Css.Classes.Select.Control}").Click();
        cut.FindAll($".{Css.Classes.Select.Option}")[1].Click();

        Assert.Null(picked);
    }

    [Fact]
    public void EnabledOption_StillCommits()
    {
        string? picked = null;
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.ChildContent, Options)
            .Add(x => x.ValueChanged, (string? v) => picked = v));

        cut.Find($".{Css.Classes.Select.Control}").Click();
        cut.FindAll($".{Css.Classes.Select.Option}")[0].Click();

        Assert.Equal("Shared", picked);
    }

    // Both sources answer, so a caller using Items AND ItemDisabled is unaffected by the new path.
    [Fact]
    public void ItemDisabledStillDecidesForTheItemsPath()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, new[] { "a", "b" })
            .Add(x => x.ItemDisabled, (string v) => v == "b"));

        cut.Find($".{Css.Classes.Select.Control}").Click();

        Assert.Equal("true", cut.FindAll($".{Css.Classes.Select.Option}")[1].GetAttribute("aria-disabled"));
    }
}

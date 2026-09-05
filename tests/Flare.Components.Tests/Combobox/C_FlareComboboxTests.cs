using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class C_FlareComboboxTests : FlareTestContext
{
    private static readonly string[] _cities = ["Berlin", "London", "Paris", "Tokyo"];

    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, _cities));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Autocomplete.Root}"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Label, "City")
            .Add(x => x.Items, _cities));

        Assert.Equal("City", cut.Find($"label.{Css.Classes.Input.Label}").TextContent);
    }

    [Fact]
    public void RendersPlaceholder()
    {
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Placeholder, "Search city...")
            .Add(x => x.Items, _cities));

        Assert.Equal("Search city...", cut.Find("input").GetAttribute("placeholder"));
    }

    [Fact]
    public void RendersDisabled()
    {
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, _cities)
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void RendersHelperText()
    {
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, _cities)
            .Add(x => x.HelperText, "Start typing to search"));

        Assert.Contains("Start typing to search", cut.Find($".{Css.Classes.Input.Helper}").TextContent);
    }

    [Fact]
    public void RendersErrorText()
    {
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, _cities)
            .Add(x => x.ErrorText, "City required"));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Input.HelperError}"));
    }

    [Fact]
    public void DropdownNotShownInitially()
    {
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, _cities));

        Assert.Empty(cut.FindAll($".{Css.Classes.Listbox.Root}"));   // the option surface is not rendered while closed
    }

    [Fact]
    public void RendersInputField()
    {
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, _cities));

        Assert.NotNull(cut.Find($"input.{Css.Classes.Input.Control}"));
    }
}

// ------------------------------------------------------------------------------
// FlareMultiSelect  (10 tests from Wave1)
// ------------------------------------------------------------------------------

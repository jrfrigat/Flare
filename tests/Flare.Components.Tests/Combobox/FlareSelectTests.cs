using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareSelectTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareSelect<string>>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Select.Root}"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Label, "My Label"));

        Assert.Contains("My Label", cut.Find($".{Css.Classes.Input.Label}").TextContent);
    }

    [Fact]
    public void RendersControl()
    {
        var cut = Render<FlareSelect<string>>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Select.Control}"));
    }

    [Fact]
    public void RendersDisabled()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Disabled, true));

        Assert.Contains(Css.Classes.Input.Disabled, cut.Find($".{Css.Classes.Select.Root}").ClassName);
    }

    [Fact]
    public void RendersHelperText()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.HelperText, "Hint text"));

        Assert.Contains("Hint text", cut.Find($".{Css.Classes.Input.Helper}").TextContent);
    }

    [Fact]
    public void RendersErrorText()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.ErrorText, "Required"));

        Assert.Contains("Required", cut.Find($".{Css.Classes.Input.HelperError}").TextContent);
    }

    [Fact]
    public void RendersOptionItems()
    {
        var items = new[] { "Alpha", "Beta", "Gamma" };
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, items));

        cut.Find($".{Css.Classes.Select.Control}").Click();

        var options = cut.FindAll($".{Css.Classes.Select.Option}");
        Assert.Equal(3, options.Count);
    }

    [Fact]
    public void AcceptsAdditionalAttributes()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .AddUnmatched("data-testid", "my-select"));

        Assert.Equal("my-select", cut.Find($".{Css.Classes.Select.Root}").GetAttribute("data-testid"));
    }

    [Fact]
    public void ItemTemplate_RendersCustomMarkup()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, new[] { "a", "b" })
            .Add(x => x.ItemTemplate, (RenderFragment<string>)(v => b => b.AddMarkupContent(0, $"<em class=\"tpl\">{v}</em>"))));

        cut.Find($".{Css.Classes.Select.Control}").Click();

        Assert.Equal(2, cut.FindAll($".{Css.Classes.Select.Option} .tpl").Count);
    }
}

// ------------------------------------------------------------------------------
// FlareCombobox  (was FlareAutocomplete; absorbed in the select-family rebuild)
// ------------------------------------------------------------------------------

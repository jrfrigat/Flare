using Flare.Infrastructure;
using Flare.Abstractions;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareDialogTests : FlareTestContext
{
    [Fact]
    public void HiddenWhenVisibleFalse()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, false));

        Assert.Empty(cut.FindAll($".{Css.Classes.Dialog.Scrim}"));
    }

    [Fact]
    public void RendersWhenVisibleTrue()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Dialog.Scrim}"));
    }

    [Fact]
    public void RendersTitle_WhenProvided()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.Title, "Confirm Action"));

        Assert.Contains("Confirm Action", cut.Find($".{Css.Classes.Dialog.Title}").TextContent);
    }

    [Fact]
    public void RendersChildContent_WhenVisible()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .AddChildContent("<p id=\"dialog-body\">Body</p>"));

        Assert.NotEmpty(cut.FindAll("#dialog-body"));
    }

    [Fact]
    public void TitledDialog_LabelledByTitle_NoAriaLabel()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.Title, "Confirm"));

        var panel = cut.Find("[role=dialog]");
        Assert.True(panel.HasAttribute("aria-labelledby"));
        Assert.False(panel.HasAttribute("aria-label"));
    }

    [Fact]
    public void HeaderlessDialog_HasNoDanglingLabelledBy_UsesAriaLabel()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.AriaLabel, "Quick action")
            .AddChildContent("<p>Body</p>"));

        var panel = cut.Find("[role=dialog]");
        // No title -> must not point aria-labelledby at a non-existent element
        Assert.False(panel.HasAttribute("aria-labelledby"));
        Assert.Equal("Quick action", panel.GetAttribute("aria-label"));
    }

    [Fact]
    public void DefaultSize_HasMdClass()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Dialog.SizeMd}"));
    }
}

// ------------------------------------------------------------------------------
// FlareAlert  (7 tests from Wave3)
// ------------------------------------------------------------------------------

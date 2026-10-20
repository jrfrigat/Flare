using Flare.Infrastructure;
using Flare.Abstractions;
using Flare.Components.Resources;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareDialogTests : FlareTestContext
{
    [Fact]
    public void DraggableHeaderAndResizerAreKeyboardReachableAndTheCloseReservesHeaderSpace()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.Title, "Title")
            .Add(x => x.Draggable, true)
            .Add(x => x.Resizable, true)
            .Add(x => x.ShowCloseButton, true));

        var header = cut.Find($".{Css.Classes.Dialog.Header}");
        Assert.Equal("0", header.GetAttribute("tabindex"));
        Assert.Equal(FlareStrings.Dialog_Move, header.GetAttribute("aria-label"));

        var resizer = cut.Find($".{Css.Classes.Dialog.Resizer}");
        Assert.Equal("0", resizer.GetAttribute("tabindex"));
        Assert.Equal("separator", resizer.GetAttribute("role"));
        Assert.Equal(FlareStrings.Dialog_Resize, resizer.GetAttribute("aria-label"));

        Assert.Contains(Css.Classes.Dialog.HasClose, cut.Find($".{Css.Classes.Dialog.Root}").ClassName);
    }

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

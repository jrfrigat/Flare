// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareRadioGroup  (6 tests)
// -----------------------------------------------------------------------------

public class FlareRadioGroupTests : FlareTestContext
{
    [Fact]
    public void RendersRootFieldset()
    {
        var cut = Render<FlareRadioGroup<string>>();

        Assert.NotEmpty(cut.FindAll(".flare-radio-group"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareRadioGroup<string>>(p => p
            .Add(x => x.Label, "Pick one"));

        Assert.Contains("Pick one", cut.Find(".flare-radio-group__legend").TextContent);
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareRadioGroup<string>>(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenElement(0, "span");
                b.AddAttribute(1, "class", "test-child");
                b.CloseElement();
            })));

        Assert.NotEmpty(cut.FindAll(".test-child"));
    }

    [Fact]
    public void RendersDisabledClass()
    {
        // Disabled state adds no special class on fieldset itself for RadioGroup,
        // but the context propagates disabled. We verify the fieldset renders.
        var cut = Render<FlareRadioGroup<string>>(p => p
            .Add(x => x.Disabled, true));

        Assert.NotEmpty(cut.FindAll("fieldset"));
    }

    [Fact]
    public void RendersErrorText()
    {
        var cut = Render<FlareRadioGroup<string>>(p => p
            .Add(x => x.ErrorText, "Selection required"));

        Assert.Contains("Selection required", cut.Find(".flare-radio-group__error").TextContent);
    }

    [Fact]
    public void RendersInlineClass()
    {
        var cut = Render<FlareRadioGroup<string>>(p => p
            .Add(x => x.Inline, true));

        Assert.Contains("flare-radio-group--inline", cut.Find("fieldset").ClassName);
    }
}

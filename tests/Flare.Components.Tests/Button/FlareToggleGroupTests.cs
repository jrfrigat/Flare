using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareToggleGroupTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareToggleGroup<string>>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.ToggleGroup.Root}"));
    }

    [Fact]
    public void Horizontal_Default_NoVerticalClass()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .Add(x => x.Orientation, "horizontal"));

        Assert.DoesNotContain(Css.Classes.ToggleGroup.Vertical, cut.Find("div").ClassName ?? "");
    }

    [Fact]
    public void Vertical_Orientation_AddsVerticalClass()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .Add(x => x.Orientation, "vertical"));

        Assert.Contains(Css.Classes.ToggleGroup.Vertical, cut.Find("div").ClassName ?? "");
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .AddChildContent("<span id=\"toggle-child\">Item</span>"));

        Assert.NotEmpty(cut.FindAll("#toggle-child"));
    }

    [Fact]
    public void MultiSelect_False_IsDefault()
    {
        var cut = Render<FlareToggleGroup<string>>();

        Assert.False(cut.Instance.MultiSelect);
    }

    [Fact]
    public void MultiSelect_True_AcceptedWithoutError()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .Add(x => x.MultiSelect, true));

        Assert.True(cut.Instance.MultiSelect);
    }

    [Fact]
    public void ChildToggleButton_RendersInsideGroup()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .AddChildContent<FlareToggleButton>(bp => bp
                .Add(x => x.Value, "A")));

        Assert.NotEmpty(cut.FindAll($"button.{Css.Classes.Button.Root}"));
    }

    [Fact]
    public void AdditionalAttributes_PassThrough()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .AddUnmatched("data-testid", "toggle-group"));

        Assert.Equal("toggle-group", cut.Find($".{Css.Classes.ToggleGroup.Root}").GetAttribute("data-testid"));
    }
}

// ------------------------------------------------------------------------------
// FlareTagField  (5 tests from Wave3)
// ------------------------------------------------------------------------------

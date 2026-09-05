using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareListItemTests : FlareTestContext
{
    [Fact]
    public void RendersLiElement()
    {
        var cut = Render<FlareListItem>();

        Assert.NotEmpty(cut.FindAll($"li.{Css.Classes.List.Item}"));
    }

    [Fact]
    public void RendersPrimaryText()
    {
        var cut = Render<FlareListItem>(p => p
            .Add(x => x.Primary, "Primary Label"));

        Assert.Contains("Primary Label", cut.Markup);
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.List.Primary}"));
    }

    [Fact]
    public void RendersSecondaryText()
    {
        var cut = Render<FlareListItem>(p => p
            .Add(x => x.Primary, "Title")
            .Add(x => x.Secondary, "Subtitle text"));

        Assert.Contains("Subtitle text", cut.Markup);
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.List.Secondary}"));
    }

    [Fact]
    public void NoPrimary_NoPrimarySpan()
    {
        var cut = Render<FlareListItem>();

        Assert.Empty(cut.FindAll($".{Css.Classes.List.Primary}"));
    }

    [Fact]
    public void Disabled_HasDisabledClass()
    {
        var cut = Render<FlareListItem>(p => p
            .Add(x => x.Disabled, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.List.Disabled}"));
    }

    [Fact]
    public void Selected_HasSelectedClass()
    {
        var cut = Render<FlareListItem>(p => p
            .Add(x => x.Selected, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.List.Selected}"));
    }

    [Fact]
    public void NotSelected_NoSelectedClass()
    {
        var cut = Render<FlareListItem>(p => p
            .Add(x => x.Selected, false));

        Assert.Empty(cut.FindAll($".{Css.Classes.List.Selected}"));
    }

    [Fact]
    public void WithClickHandler_HasClickableClass()
    {
        var cut = Render<FlareListItem>(p => p
            .Add(x => x.OnClick, EventCallback.Factory.Create(this, () => { })));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.List.Clickable}"));
    }
}

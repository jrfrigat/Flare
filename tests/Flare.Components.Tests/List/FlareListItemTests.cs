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

    // A clickable row with its own action in a slot (Merito: a delete button on a catalogue row). The
    // button must act alone, by mouse and by keyboard, while the rest of the row still opens it.
    // Keys are checked in Flare.Browser.Tests: bUnit bubbles keydown past a stopPropagation that a browser honours.
    private (IRenderedComponent<FlareListItem> Cut, Func<int> RowClicks, Func<int> ButtonClicks) RowWithSlotButton(bool trailing)
    {
        var rowClicks = 0;
        var buttonClicks = 0;
        RenderFragment button = b =>
        {
            b.OpenElement(0, "button");
            b.AddAttribute(1, "id", "slot-action");
            b.AddAttribute(2, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => buttonClicks++));
            b.CloseElement();
        };
        var cut = Render<FlareListItem>(p => p
            .Add(x => x.Primary, "Fine")
            .Add(x => x.OnClick, EventCallback.Factory.Create(this, () => rowClicks++))
            .Add(trailing ? x => x.Trailing! : x => x.Leading!, button));
        return (cut, () => rowClicks, () => buttonClicks);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SlotButtonClick_DoesNotActivateTheRow(bool trailing)
    {
        var (cut, rowClicks, buttonClicks) = RowWithSlotButton(trailing);

        cut.Find("#slot-action").Click();

        Assert.Equal(1, buttonClicks());
        Assert.Equal(0, rowClicks());
    }

    [Fact]
    public void ContentClick_StillActivatesTheRow()
    {
        var (cut, rowClicks, _) = RowWithSlotButton(trailing: true);

        cut.Find($".{Css.Classes.List.Primary}").Click();

        Assert.Equal(1, rowClicks());
    }
}

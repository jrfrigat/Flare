using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;

namespace Flare.Components.Tests;

/// <summary>
/// The card is activated by a native control rather than a key handler on a div: that is what makes
/// Enter and Space activate it once and Space stop scrolling the page, and it keeps the card's content
/// out of the control, so nested interactive elements are independent.
/// </summary>
public class FlareCardKeyboardActivationTests : FlareTestContext
{
    [Fact]
    public void ClickableCard_ActivatesThroughANativeButton()
    {
        var clicks = 0;
        var cut = Render<FlareCard>(p => p
            .Add(x => x.OnClick, () => clicks++)
            .AddChildContent("Card"));

        cut.Find($"button.{Css.Classes.Card.Hit}").Click();

        Assert.Equal(1, clicks);
    }

    [Fact]
    public void TheActivationControl_IsAButtonOutsideTheTabOrderOfTheRoot()
    {
        var cut = Render<FlareCard>(p => p
            .Add(x => x.OnClick, () => { })
            .AddChildContent("Card"));

        var root = cut.Find($".{Css.Classes.Card.Root}");
        Assert.Null(root.GetAttribute("tabindex"));
        Assert.Null(root.GetAttribute("onkeydown"));

        var hit = cut.Find($"button.{Css.Classes.Card.Hit}");
        Assert.Equal("button", hit.GetAttribute("type"));
    }

    [Fact]
    public void NonInteractiveCard_HasNoButtonRole()
    {
        var cut = Render<FlareCard>(p => p.AddChildContent("Card"));

        Assert.Null(cut.Find($".{Css.Classes.Card.Root}").GetAttribute("role"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Card.Hit}"));
    }
}

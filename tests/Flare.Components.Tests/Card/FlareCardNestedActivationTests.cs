using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// The card's activation control is an empty sibling layer, not an ancestor of the content, so a nested
/// control answers its own click and its own Enter/Space while the card stays out of the way.
/// </summary>
public class FlareCardNestedActivationTests : FlareTestContext
{
    private static string HitSelector => $".{Css.Classes.Card.Hit}";

    private RenderFragment NestedButton(string id, Action onClick) => b =>
    {
        b.OpenElement(0, "button");
        b.AddAttribute(1, "id", id);
        b.AddAttribute(2, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, _ => onClick()));
        b.AddContent(3, "Inner action");
        b.CloseElement();
    };

    private RenderFragment NestedLink(string id, Action onClick) => b =>
    {
        b.OpenElement(0, "a");
        b.AddAttribute(1, "id", id);
        b.AddAttribute(2, "href", "/inner");
        b.AddAttribute(3, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, _ => onClick()));
        b.AddContent(4, "Inner link");
        b.CloseElement();
    };

    private RenderFragment NestedInput(string id, Action onKeyDown) => b =>
    {
        b.OpenElement(0, "input");
        b.AddAttribute(1, "id", id);
        b.AddAttribute(2, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, _ => onKeyDown()));
        b.CloseElement();
    };

    [Fact]
    public void ClickingANestedButton_DoesNotActivateTheCard()
    {
        var card = 0;
        var inner = 0;
        var cut = Render<FlareCard>(p => p
            .Add(x => x.OnClick, () => card++)
            .Add(x => x.ChildContent, NestedButton("inner", () => inner++)));

        cut.Find("#inner").Click();

        Assert.Equal(1, inner);
        Assert.Equal(0, card);
    }

    [Fact]
    public void ClickingANestedLink_DoesNotActivateTheCard()
    {
        var card = 0;
        var inner = 0;
        var cut = Render<FlareCard>(p => p
            .Add(x => x.OnClick, () => card++)
            .Add(x => x.ChildContent, NestedLink("inner", () => inner++)));

        cut.Find("#inner").Click();

        Assert.Equal(1, inner);
        Assert.Equal(0, card);
    }

    [Fact]
    public void ClickingANestedButton_DoesNotToggleASelectableCard()
    {
        bool? changed = null;
        var cut = Render<FlareCard>(p => p
            .Add(x => x.Selectable, true)
            .Add(x => x.OnSelectionChange, EventCallback.Factory.Create<bool>(this, v => changed = v))
            .Add(x => x.ChildContent, NestedButton("inner", () => { })));

        cut.Find("#inner").Click();

        Assert.Null(changed);
    }

    [Theory]
    [InlineData("Enter")]
    [InlineData(" ")]
    public void EnterOrSpaceInANestedInput_DoesNotActivateTheCard(string key)
    {
        var card = 0;
        var keys = 0;
        var cut = Render<FlareCard>(p => p
            .Add(x => x.OnClick, () => card++)
            .Add(x => x.ChildContent, NestedInput("inner", () => keys++)));

        cut.Find("#inner").KeyDown(new KeyboardEventArgs { Key = key });

        Assert.Equal(1, keys);
        Assert.Equal(0, card);
    }

    [Fact]
    public void ClickingTheActivationControl_ActivatesTheCardOnce()
    {
        var card = 0;
        var cut = Render<FlareCard>(p => p
            .Add(x => x.OnClick, () => card++)
            .Add(x => x.ChildContent, NestedButton("inner", () => { })));

        cut.Find(HitSelector).Click();

        Assert.Equal(1, card);
    }

    [Fact]
    public void TheActivationControl_IsNamedByTheCardContent()
    {
        var cut = Render<FlareCard>(p => p
            .Add(x => x.OnClick, () => { })
            .AddChildContent("Mountain retreat"));

        var root = cut.Find($".{Css.Classes.Card.Root}");
        var hit = cut.Find($"button{HitSelector}");

        Assert.False(string.IsNullOrEmpty(root.Id));
        Assert.Equal(root.Id, hit.GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void ConsumerAriaLabel_NamesTheActivationControl()
    {
        var cut = Render<FlareCard>(p => p
            .AddUnmatched("aria-label", "Open mountain retreat")
            .Add(x => x.OnClick, () => { }));

        var hit = cut.Find($"button{HitSelector}");

        Assert.Equal("Open mountain retreat", hit.GetAttribute("aria-label"));
        Assert.Null(hit.GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void LinkCard_KeepsContentOutOfTheAnchor()
    {
        var cut = Render<FlareCard>(p => p
            .Add(x => x.Href, "/target")
            .Add(x => x.ChildContent, NestedButton("inner", () => { })));

        var root = cut.Find($".{Css.Classes.Card.Root}");
        var hit = cut.Find($"a{HitSelector}");

        Assert.Equal("DIV", root.TagName);
        Assert.Equal("/target", hit.GetAttribute("href"));
        Assert.Equal(root.Id, hit.GetAttribute("aria-labelledby"));
        // The content is a sibling of the card's anchor, not inside it: no interactive anchor wraps it.
        Assert.Empty(hit.QuerySelectorAll("button"));
        Assert.Single(cut.FindAll("#inner"));
    }
}

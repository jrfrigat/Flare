using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// The body used to be a span with role=button that also held the close button: Enter/Space on close
/// toggled the chip first, and Space on the body scrolled the page. The body action is now a native
/// button and the close button is its sibling, so the browser own button behaviour applies.
/// </summary>
public class FlareChipKeyboardTests : FlareTestContext
{
    [Fact]
    public void CloseButton_IsNotInsideTheAction()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.OnClick, EventCallback.Factory.Create(this, () => { }))
            .Add(x => x.Closeable, true));

        var action = cut.Find($"button.{Css.Classes.Chip.Action}");
        Assert.Null(action.QuerySelector($"button.{Css.Classes.Chip.Close}"));
        Assert.NotNull(cut.Find($"button.{Css.Classes.Chip.Close}"));
    }

    // Enter and Space are the browser's own button behaviour: Space does not scroll, and no key
    // handler in C# has to exist for the chip body.
    [Fact]
    public void BodyAction_IsANativeButton()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.SelectedChanged, (bool _) => { }));

        var action = cut.Find($".{Css.Classes.Chip.Action}");

        Assert.Equal("BUTTON", action.TagName);
        Assert.Null(action.GetAttribute("tabindex"));
    }

    [Fact]
    public void ClickingTheAction_TogglesOnce()
    {
        var selections = 0;
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.SelectedChanged, (bool _) => selections++));

        cut.Find($"button.{Css.Classes.Chip.Action}").Click();

        Assert.Equal(1, selections);
    }

    [Fact]
    public void ActionButton_IsNamedByTheLabel()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Filter")
            .Add(x => x.OnClick, EventCallback.Factory.Create(this, () => { })));

        var action = cut.Find($"button.{Css.Classes.Chip.Action}");
        var labelId = action.GetAttribute("aria-labelledby");

        Assert.False(string.IsNullOrEmpty(labelId));
        Assert.Equal("Filter", cut.Find($"#{labelId}").TextContent);
    }
}

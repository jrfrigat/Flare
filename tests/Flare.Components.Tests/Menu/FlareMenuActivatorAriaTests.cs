using AngleSharp.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// A menu button announces itself through the element that takes the focus. The menu used to write
/// <c>aria-haspopup</c>, <c>aria-expanded</c> and <c>aria-controls</c> on the wrapper around the
/// activator - a div with no role, which announces nothing - so a screen reader said "button, More
/// actions" and nothing about there being a menu or about it being open. The attributes now travel to
/// the caller through the activator context, and these tests hold both halves of that: they arrive on
/// the caller's button, and the wrapper no longer pretends to carry them.
/// </summary>
public sealed class FlareMenuActivatorAriaTests : FlareTestContext
{
    // A catch-all module handler rather than the overlay module by URL: that URL holds a CSS name the
    // registry owns, and a test may not spell one of those out. focusFirstInDialog is the menu's only
    // call on it either way.

    private static RenderFragment<FlareMenuActivatorContext> ActivatorButton() => ctx => b =>
    {
        b.OpenElement(0, "button");
        b.AddMultipleAttributes(1, ctx.Attributes);
        b.AddContent(2, "open");
        b.CloseElement();
    };

    private static RenderFragment OneItem() => b =>
    {
        b.OpenComponent<FlareMenuItem>(0);
        b.AddAttribute(1, nameof(FlareMenuItem.ChildContent), (RenderFragment)(c => c.AddContent(0, "Save")));
        b.CloseComponent();
    };

    private IRenderedComponent<FlareMenu> RenderMenu(
        RenderFragment<FlareMenuActivatorContext>? activator = null) =>
        Render<FlareMenu>(p => p
            .Add(m => m.Activator, activator ?? ActivatorButton())
            .Add(m => m.ChildContent, OneItem()));

    private static IElement Activator(IRenderedComponent<FlareMenu> cut) =>
        cut.Find($".{Css.Classes.Menu.Activator} button");

    [Fact]
    public void TheAriaArrivesOnTheActivatorsOwnButton()
    {
        var cut = RenderMenu();

        var button = Activator(cut);
        Assert.Equal("menu", button.GetAttribute("aria-haspopup"));
        Assert.Equal("false", button.GetAttribute("aria-expanded"));
        Assert.False(string.IsNullOrEmpty(button.GetAttribute("aria-controls")));
    }

    // The wrapper is where all three used to live. Leaving them there as well would keep announcing a
    // popup on an element that has no role and cannot be reached, so it has to be clean.
    [Fact]
    public void TheWrapperCarriesNoneOfThem()
    {
        var cut = RenderMenu();

        var wrapper = cut.Find($".{Css.Classes.Menu.Activator}");
        Assert.False(wrapper.HasAttribute("aria-haspopup"));
        Assert.False(wrapper.HasAttribute("aria-expanded"));
        Assert.False(wrapper.HasAttribute("aria-controls"));
    }

    [Fact]
    public async Task OpeningFlipsAriaExpandedAndPointsAriaControlsAtThePanel()
    {
        var cut = RenderMenu();

        await cut.InvokeAsync(() => Activator(cut).Click(new MouseEventArgs { Detail = 1 }));

        var button = Activator(cut);
        Assert.Equal("true", button.GetAttribute("aria-expanded"));
        Assert.Equal(cut.Find("[role=menu]").Id, button.GetAttribute("aria-controls"));
    }

    // Enter and Space on a <button> raise a click carrying no click count, which is how the menu tells a
    // keyboard opening from a pointer one - and a keyboard opening is the one that has to start with an
    // item highlighted, because the keyboard user has nowhere else to look.
    [Fact]
    public async Task TheKeyboardOpensTheMenuThroughThatButtonWithTheFirstItemHighlighted()
    {
        var cut = RenderMenu();

        await cut.InvokeAsync(() => Activator(cut).Click(new MouseEventArgs { Detail = 0 }));

        Assert.NotEmpty(cut.FindAll("[role=menu]"));
        Assert.False(string.IsNullOrEmpty(cut.Find("[role=menu]").GetAttribute("aria-activedescendant")));
    }

    // Closing the panel takes the focus with it: the panel held it, and the panel is gone. Without
    // handing it back the focus falls to the document body, and the keyboard user resumes from the top
    // of the page rather than from the button they just pressed.
    [Fact]
    public async Task EscapeClosesTheMenuAndSendsTheFocusBackToTheActivator()
    {
        var module = JSInterop.SetupModule();
        var cut = RenderMenu();

        await cut.InvokeAsync(() => Activator(cut).Click(new MouseEventArgs { Detail = 0 }));
        Assert.Empty(module.Invocations["focusFirstInDialog"]);   // not while it is open

        await cut.InvokeAsync(() => cut.Find("[role=menu]").KeyDown(new KeyboardEventArgs { Key = "Escape" }));

        Assert.Empty(cut.FindAll("[role=menu]"));
        Assert.Single(module.Invocations["focusFirstInDialog"]);
    }

    // The caret is the only part of a split button that opens the menu, so it is the only part that may
    // claim a popup - the primary action must not look like one.
    [Fact]
    public void ASplitButtonPutsTheMenuAriaOnItsCaretAndNotOnTheAction()
    {
        var cut = Render<FlareSplitButton>(p => p
            .Add(x => x.ChildContent, b => b.AddMarkupContent(0, "Save"))
            .Add(x => x.MenuItems, OneItem()));

        Assert.Equal("menu", cut.Find($"button.{Css.Classes.SplitButton.Trigger}").GetAttribute("aria-haspopup"));
        Assert.False(cut.Find($"button.{Css.Classes.SplitButton.Main}").HasAttribute("aria-haspopup"));
    }

    // Open is what an activator dresses itself with - a turned chevron, a pressed look - so it has to
    // move with the panel rather than being a snapshot of the first render.
    [Fact]
    public async Task TheContextTellsTheActivatorWhetherTheMenuIsOpen()
    {
        RenderFragment<FlareMenuActivatorContext> chevron = ctx => b =>
        {
            b.OpenElement(0, "button");
            b.AddMultipleAttributes(1, ctx.Attributes);
            b.AddAttribute(2, "data-open", ctx.Open ? "yes" : "no");
            b.AddContent(3, "open");
            b.CloseElement();
        };

        var cut = RenderMenu(chevron);
        Assert.Equal("no", Activator(cut).GetAttribute("data-open"));

        await cut.InvokeAsync(() => Activator(cut).Click(new MouseEventArgs { Detail = 1 }));

        Assert.Equal("yes", Activator(cut).GetAttribute("data-open"));
    }
}

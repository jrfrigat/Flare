using AngleSharp.Dom;
using Flare.Components;
using Flare.Integration.Tests.Screens;
using Microsoft.AspNetCore.Components;

namespace Flare.Integration.Tests;

/// <summary>
/// The same form, two levels down inside something else - a tab panel and a dialog. A form is the one
/// composition where nesting can break it invisibly: the submit button reaches its form through the
/// DOM, the fields reach their EditContext through the cascade, and a container between them can cut
/// either without any component being wrong on its own.
/// </summary>
public class NestedFormTests : AppScreenContext
{
    // The subtree the form lives in on each screen. Every lookup is scoped to it and re-run from the
    // current tree, for two reasons the composed screen creates and a lone form does not: the tab
    // screen has OTHER text inputs on the tab that is not showing (the grid's filter row), so a
    // document-wide "the text input" fills in the wrong one; and each interaction re-renders, which
    // leaves a previously found element carrying event handler ids the new tree no longer has.
    private const string TabPanel = "." + Css.Classes.Tabs.Panel + ":not(." + Css.Classes.Tabs.PanelHidden + ")";
    private const string Dialog = "[role=dialog]";

    private readonly NewOrder _model = new();
    private NewOrder? _submitted;

    [Fact]
    public void TheFormSubmitsFromInsideACardInsideATab()
    {
        var screen = Render<OrdersScreen>(p => p
            .Add(x => x.InitialTab, 1)
            .Add(x => x.Draft, _model)
            .Add(x => x.OnOrderPlaced, o => _submitted = o));

        Fill(screen, TabPanel);
        In(screen, TabPanel, "button[type=submit]").Click();

        Assert.Same(_model, _submitted);
        Assert.Equal("Aster", _submitted!.Customer);
    }

    [Fact]
    public void TheFormSubmitsFromInsideADialogAndTheDialogCloses()
    {
        var screen = Render<NewOrderDialog>(p => p
            .Add(x => x.Model, _model)
            .Add(x => x.OnSubmitted, o => _submitted = o));

        screen.Find($".{Css.Classes.Button.Root}").Click();
        Assert.NotEmpty(screen.FindAll(Dialog));

        Fill(screen, Dialog);
        In(screen, Dialog, "button[type=submit]").Click();

        Assert.Same(_model, _submitted);
        Assert.Empty(screen.FindAll(Dialog));
    }

    [Fact]
    public void ARefusedSubmitLeavesTheDialogOpenWithTheReason()
    {
        // The other half: a dialog that closed on any submit would throw the user's half-filled draft
        // away and never show them why it was refused.
        var screen = Render<NewOrderDialog>(p => p
            .Add(x => x.Model, _model)
            .Add(x => x.OnSubmitted, o => _submitted = o));

        screen.Find($".{Css.Classes.Button.Root}").Click();
        In(screen, Dialog, "button[type=submit]").Click();

        Assert.Null(_submitted);
        Assert.NotEmpty(screen.FindAll(Dialog));
        Assert.Contains("Customer is required",
            string.Concat(screen.FindAll($".{Css.Classes.Validation.SummaryItem}").Select(li => li.TextContent)));
    }

    private void Fill<T>(IRenderedComponent<T> screen, string scope) where T : IComponent
    {
        In(screen, scope, "input[type=text]").Change("Aster");
        In(screen, scope, $".{Css.Classes.Select.Control}").Click();
        All(screen, scope, $".{Css.Classes.Listbox.Option}")
            .First(o => o.TextContent.Contains("Standard", StringComparison.Ordinal))
            .Click();
        In(screen, scope, "input[type=number]").Change("2");
        In(screen, scope, "input[type=checkbox]").Change(true);
    }

    private static IElement In<T>(IRenderedComponent<T> screen, string scope, string selector) where T : IComponent =>
        All(screen, scope, selector).First();

    private static IEnumerable<IElement> All<T>(IRenderedComponent<T> screen, string scope, string selector)
        where T : IComponent =>
        screen.FindAll($"{scope} {selector}");
}

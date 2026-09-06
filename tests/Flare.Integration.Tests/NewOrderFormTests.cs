using AngleSharp.Dom;
using Flare.Components;
using Flare.Integration.Tests.Screens;

namespace Flare.Integration.Tests;

/// <summary>
/// A form that submits, filled the way a user fills it: type in the field, open the select and pick a
/// row, set the number, tick the box, press the button. Four field families, one EditContext and one
/// model - and none of the per-component suites can say whether a submit collects all four, because
/// each of them renders its component alone with no form around it.
/// </summary>
public class NewOrderFormTests : AppScreenContext
{
    private NewOrder _model = new();
    private NewOrder? _submitted;
    private int _rejected;

    private IRenderedComponent<NewOrderForm> Form()
    {
        _model = new NewOrder();
        return Render<NewOrderForm>(p => p
            .Add(x => x.Model, _model)
            .Add(x => x.OnSubmitted, o => _submitted = o)
            .Add(x => x.OnRejected, () => _rejected++));
    }

    [Fact]
    public void AnEmptyFormIsRefusedAndSaysWhy()
    {
        var form = Form();

        Submit(form);

        Assert.Null(_submitted);
        Assert.Equal(1, _rejected);

        // The summary is the form's answer, and it must name every broken rule at once - a form that
        // reports one failure per submit makes the user press the button four times.
        var messages = form.FindAll($".{Css.Classes.Validation.SummaryItem}")
            .Select(li => li.TextContent).ToList();
        Assert.Contains("Customer is required", messages);
        Assert.Contains("Shipping is required", messages);
        Assert.Contains("The terms must be accepted", messages);
    }

    [Fact]
    public void FillingEveryFieldSubmitsTheModel()
    {
        var form = Form();

        Fill(form);
        Submit(form);

        Assert.Same(_model, _submitted);
        Assert.Equal("Aster", _submitted!.Customer);
        Assert.Equal("Express", _submitted.Shipping);
        Assert.Equal(3, _submitted.Quantity);
        Assert.True(_submitted.Accepted);
        Assert.Equal(0, _rejected);
    }

    [Fact]
    public void FixingTheOffendingFieldClearsItsMessageAndLeavesTheRest()
    {
        // The point of one EditContext behind four components: filling one of them revalidates that
        // field and only that field, so the summary shrinks rather than resetting.
        var form = Form();
        Submit(form);
        Assert.Contains("Customer is required", SummaryText(form));

        form.Find("input[type=text]").Change("Aster");

        Assert.DoesNotContain("Customer is required", SummaryText(form));
        Assert.Contains("Shipping is required", SummaryText(form));
    }

    [Fact]
    public void TheFieldThatFailedIsMarkedInvalid()
    {
        // Not only the summary: the field itself has to show it, or a long form makes the user match
        // messages to inputs by hand.
        var form = Form();
        Submit(form);

        var customer = form.Find("input[type=text]");
        Assert.Equal("true", customer.GetAttribute("aria-invalid"));
    }

    // ---- driving the form the way a user does -------------------------------------------------

    private static void Fill(IRenderedComponent<NewOrderForm> form)
    {
        form.Find("input[type=text]").Change("Aster");

        // The select is a listbox, not a <select>: open it, then click the row.
        form.Find($".{Css.Classes.Select.Control}").Click();
        form.FindAll($".{Css.Classes.Listbox.Option}")
            .First(o => o.TextContent.Contains("Express", StringComparison.Ordinal))
            .Click();

        form.Find("input[type=number]").Change("3");
        form.Find("input[type=checkbox]").Change(true);
    }

    private static void Submit(IRenderedComponent<NewOrderForm> form) =>
        form.Find("button[type=submit]").Click();

    private static string SummaryText(IRenderedComponent<NewOrderForm> form) =>
        string.Concat(form.FindAll($".{Css.Classes.Validation.SummaryItem}").Select(li => li.TextContent));
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Flare.Components.Tests;

/// <summary>
/// The other half of the button-type guard: the markup scan proves every <c>&lt;button&gt;</c> declares
/// a type, and this proves what that buys - pressing a component's own command button inside an
/// <c>EditForm</c> does not submit the form.
/// </summary>
/// <remarks>
/// A button with no <c>type</c> is a submit button by default, so a calendar's month arrow, a pagination
/// page, a tab or a stepper's next button would post the form around it. The failure is quiet in
/// exactly the way that hurts: the control does its job, and the form submits as well.
/// </remarks>
public class NoAccidentalFormSubmitTests : FlareTestContext
{
    private sealed class Model { public string? Text { get; set; } }

    /// <summary>
    /// Renders <paramref name="content"/> inside an EditForm, presses EVERY button it draws, and returns
    /// how many times the form submitted. Every button rather than one: which button sits at which index
    /// depends on state the test does not control, and a single index quietly covers only whatever
    /// happened to be first.
    /// </summary>
    private int SubmitsWhenClickingEveryButton(RenderFragment content)
    {
        var submits = 0;

        var cut = Render<EditForm>(p => p
            .Add(f => f.Model, new Model())
            .Add(f => f.OnSubmit, EventCallback.Factory.Create<EditContext>(this, () => submits++))
            .Add(f => f.ChildContent, (RenderFragment<EditContext>)(_ => content)));

        var count = cut.FindAll("button").Count;
        Assert.True(count > 0, "The component drew no button, so this proves nothing.");

        for (var i = 0; i < count; i++)
        {
            // Re-found each time: a click can re-render and invalidate the element that was held.
            var buttons = cut.FindAll("button");
            if (i >= buttons.Count) break;
            if (buttons[i].HasAttribute("disabled")) continue;
            buttons[i].Click();
        }

        return submits;
    }

    [Fact]
    public void Pagination_PageButtonDoesNotSubmit()
        => Assert.Equal(0, SubmitsWhenClickingEveryButton(b =>
        {
            b.OpenComponent<FlarePagination>(0);
            b.AddAttribute(1, nameof(FlarePagination.TotalPages), 5);
            b.AddAttribute(2, nameof(FlarePagination.Current), 1);
            b.CloseComponent();
        }));

    [Fact]
    public void Calendar_MonthArrowDoesNotSubmit()
        => Assert.Equal(0, SubmitsWhenClickingEveryButton(b =>
        {
            b.OpenComponent<FlareCalendar>(0);
            b.CloseComponent();
        }));

    [Fact]
    public void ColorModeToggle_DoesNotSubmit()
        => Assert.Equal(0, SubmitsWhenClickingEveryButton(b =>
        {
            b.OpenComponent<FlareColorModeToggle>(0);
            b.CloseComponent();
        }));

    // FlareButton was always right - it renders its ButtonType, which defaults to Button - and this
    // keeps that true beside the raw ones rather than assuming it.
    [Fact]
    public void FlareButton_DoesNotSubmitByDefault()
        => Assert.Equal(0, SubmitsWhenClickingEveryButton(b =>
        {
            b.OpenComponent<FlareButton>(0);
            // A no-op handler only so bUnit has an onclick to dispatch; what is under test is whether the
            // FORM submits, which is decided by the button type rather than by this.
            b.AddAttribute(1, nameof(FlareButton.OnClick), EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, _ => { }));
            b.AddAttribute(2, nameof(FlareButton.ChildContent), (RenderFragment)(c => c.AddContent(0, "Command")));
            b.CloseComponent();
        }));

    // And still submits when the caller asks for it, which is what the default is measured against:
    // "no accidental submit" must not have become "no submit at all".
    [Fact]
    public void FlareButton_SubmitsWhenTypeIsSubmit()
        => Assert.Equal(1, SubmitsWhenClickingEveryButton(b =>
        {
            b.OpenComponent<FlareButton>(0);
            b.AddAttribute(1, nameof(FlareButton.Type), ButtonType.Submit);
            b.AddAttribute(2, nameof(FlareButton.OnClick), EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, _ => { }));
            b.AddAttribute(3, nameof(FlareButton.ChildContent), (RenderFragment)(c => c.AddContent(0, "Save")));
            b.CloseComponent();
        }));

}

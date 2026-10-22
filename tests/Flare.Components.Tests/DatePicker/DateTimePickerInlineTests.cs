using System.Globalization;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-663: the combined picker can render its calendar and time pane in the page instead of a popup.
/// There is no toggle, no scrim and no dialog; OK commits and the panel stays.
/// </summary>
public class DateTimePickerInlineTests : FlareTestContext
{
    private static readonly CultureInfo Ru = new("ru-RU");
    private static readonly DateTimeOffset Initial = new(2026, 10, 15, 12, 30, 0, TimeSpan.FromHours(3));

    private IRenderedComponent<FlareDateTimePicker> RenderInline(
        DateTimeOffset? value, Action<DateTimeOffset?>? changed = null) => RenderPicker(value, changed);

    private IRenderedComponent<FlareDateTimePicker> RenderPicker(
        DateTimeOffset? value, Action<DateTimeOffset?>? changed = null) =>
        Render<FlareDateTimePicker>(p => p
            .Add(x => x.Culture, Ru)
            .Add(x => x.Inline, true)
            .Add(x => x.Mode, DateTimeVariant.Panels)
            .Add(x => x.Value, value)
            .Add(x => x.ValueChanged, (DateTimeOffset? v) => changed?.Invoke(v)));

    private static void Push(IRenderedComponent<FlareDateTimePicker> cut, DateTimeOffset? value) =>
        cut.Render(p => p
            .Add(x => x.Culture, Ru)
            .Add(x => x.Inline, true)
            .Add(x => x.Mode, DateTimeVariant.Panels)
            .Add(x => x.Value, value)
            .Add(x => x.ValueChanged, (DateTimeOffset? _) => { }));

    private static string DayLabel(int day) => new DateOnly(2026, 10, day).ToString("D", Ru);

    [Fact]
    public void Inline_RendersThePanelWithoutAToggleOrAScrim()
    {
        var cut = RenderInline(Initial);

        Assert.Empty(cut.FindAll($".{Css.Classes.Input.Toggle}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Picker.Scrim}"));
        Assert.Empty(cut.FindAll("[role=dialog]"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Picker.PanelInline}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Picker.Grid}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DateTimePicker.Footer}"));
    }

    [Fact]
    public void Ok_CommitsAndThePanelStaysInPlace()
    {
        DateTimeOffset? committed = null;
        var cut = RenderInline(Initial, v => committed = v);

        cut.Find($"[aria-label='{DayLabel(20)}']").Click();
        cut.FindAll($".{Css.Classes.DateTimePicker.Footer} button").Last().Click();

        Assert.Equal(new DateTimeOffset(2026, 10, 20, 12, 30, 0, TimeSpan.FromHours(3)), committed);
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Picker.PanelInline}"));
    }

    [Fact]
    public void ExternalValueChange_ReloadsTheDraft()
    {
        var cut = RenderInline(Initial);

        Push(cut, Initial.AddDays(2));

        Assert.Equal(DayLabel(17), cut.FindAll($".{Css.Classes.Picker.Day}[aria-selected='true']").Single().GetAttribute("aria-label"));
    }

    [Fact]
    public void Escape_DoesNotRemoveTheInlinePanel()
    {
        var cut = RenderInline(Initial);

        cut.Find($".{Css.Classes.Picker.PanelInline}").KeyDown(new KeyboardEventArgs { Key = "Escape" });

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Picker.PanelInline}"));
    }
}

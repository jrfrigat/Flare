using System.Globalization;
using Bunit;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-121: keyboard editing scenarios run through the real pickers with a stubbed caret service, so
/// the mask, the neighbouring segments, ValueChanged and blur are checked together rather than the caret
/// arithmetic alone.
/// </summary>
public class MaskedEditingScenarioTests : FlareTestContext
{
    private static readonly CultureInfo Ru = CultureInfo.GetCultureInfo("ru-RU");

    private SelectionStub UseStub(TaskCompletionSource<int[]?>? pending = null)
    {
        var stub = new SelectionStub(pending);
        Services.AddSingleton<IElementJsService>(stub);
        return stub;
    }

    private IRenderedComponent<FlareDatePicker> DatePicker(Action<DateOnly?> changed, DateOnly? value = null)
        => Render<FlareDatePicker>(p => p
            .Add(x => x.Culture, Ru)
            .Add(x => x.Value, value ?? new DateOnly(2026, 10, 15))
            .Add(x => x.ValueChanged, (DateOnly? v) => changed(v)));

    [Fact]
    public void ReplacingADayDigit_RestoresTheOtherSegments()
    {
        UseStub();
        DateOnly? committed = null;
        var cut = DatePicker(v => committed = v);
        cut.Find("input").Focus();

        cut.Find("input").Input("1.10.2026");     // Backspace after "15": the digits shift
        Assert.Null(committed);
        cut.Find("input").Input("161.02.026");    // the replacement digit puts month and year back

        Assert.Equal(new DateOnly(2026, 10, 16), committed);
    }

    [Fact]
    public void DeleteInsideTheMonth_DoesNotCommit_AndBlurRestoresTheValue()
    {
        UseStub();
        var changes = 0;
        var cut = DatePicker(_ => changes++);
        cut.Find("input").Focus();

        cut.Find("input").Input("15.0.2026");     // Delete removed the month's "1"
        cut.Find("input").Change("15.02.026");
        cut.Find("input").Blur();

        Assert.Equal(0, changes);
        Assert.Equal("15.10.2026", cut.Find("input").GetAttribute("value"));
    }

    [Theory]
    [InlineData("00:00", true)]
    [InlineData("23:59", true)]
    [InlineData("24:00", false)]
    [InlineData("12:60", false)]
    public void TimeBoundaries_CommitOnlyRealTimes(string typed, bool commits)
    {
        UseStub();
        TimeOnly? committed = null;
        var changes = 0;
        var cut = Render<FlareTimePicker>(p => p
            .Add(x => x.Value, new TimeOnly(10, 0))
            .Add(x => x.ValueChanged, (TimeOnly? v) => { committed = v; changes++; }));

        cut.Find("input").Input(typed);

        Assert.Equal(commits ? 1 : 0, changes);
        if (commits) Assert.Equal(TimeOnly.ParseExact(typed, "HH:mm", CultureInfo.InvariantCulture), committed);
    }

    [Fact]
    public void PasteWithForeignSeparators_Commits()
    {
        UseStub();
        DateOnly? committed = null;
        var cut = DatePicker(v => committed = v);
        cut.Find("input").Focus();

        cut.Find("input").Input("20/11/2026");

        Assert.Equal(new DateOnly(2026, 11, 20), committed);
        Assert.Equal("20.11.2026", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Clearing_CommitsNull()
    {
        UseStub();
        DateOnly? committed = new DateOnly(1, 1, 1);
        var cut = DatePicker(v => committed = v);
        cut.Find("input").Focus();

        cut.Find("input").Input("");
        cut.Find("input").Change("");

        Assert.Null(committed);
    }

    [Fact]
    public void UndoBackToTheFullText_Commits()
    {
        UseStub();
        DateOnly? committed = null;
        var cut = DatePicker(v => committed = v);
        cut.Find("input").Focus();

        cut.Find("input").Input("15.10.202");     // a digit removed
        cut.Find("input").Input("15.10.2026");    // Ctrl+Z puts it back

        Assert.Equal(new DateOnly(2026, 10, 15), committed);
    }

    [Fact]
    public async Task SelectionAnsweredAfterBlur_LeavesTheDateFieldAlone()
    {
        var pending = new TaskCompletionSource<int[]?>(TaskCreationOptions.RunContinuationsAsynchronously);
        var stub = UseStub(pending);
        var cut = DatePicker(_ => { });
        cut.Find("input").Focus();

        var input = cut.Find("input").InputAsync(new ChangeEventArgs { Value = "1.10.2026" });
        cut.Find("input").Blur();
        pending.SetResult(new[] { 1, 1 });
        await input;

        Assert.Equal(0, stub.CaretWrites);
    }

    [Fact]
    public async Task SelectionAnsweredAfterBlur_LeavesTheTimeFieldAlone()
    {
        var pending = new TaskCompletionSource<int[]?>(TaskCreationOptions.RunContinuationsAsynchronously);
        var stub = UseStub(pending);
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.Value, new TimeOnly(12, 33)));
        cut.Find("input").Focus();

        var input = cut.Find("input").InputAsync(new ChangeEventArgs { Value = "1:33" });
        cut.Find("input").Blur();
        pending.SetResult(new[] { 1, 1 });
        await input;

        Assert.Equal(0, stub.CaretWrites);
    }

    [Fact]
    public async Task SelectionAnsweredAfterBlur_LeavesTheDateTimeFieldAlone()
    {
        var pending = new TaskCompletionSource<int[]?>(TaskCreationOptions.RunContinuationsAsynchronously);
        var stub = UseStub(pending);
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateTimeOffset(2026, 10, 15, 12, 30, 0, TimeSpan.FromHours(3))));
        cut.Find("input").Focus();

        var input = cut.Find("input").InputAsync(new ChangeEventArgs { Value = "1.10.2026 12:30" });
        cut.Find("input").Blur();
        pending.SetResult(new[] { 1, 1 });
        await input;

        Assert.Equal(0, stub.CaretWrites);
    }

    // Answers the caret read with [1, 1] (or with a pending task) and counts every write back to the field.
    private sealed class SelectionStub(TaskCompletionSource<int[]?>? pending) : IElementJsService
    {
        public int CaretWrites { get; private set; }

        public ValueTask<int[]?> GetSelectionAsync(ElementReference element) =>
            pending is null ? ValueTask.FromResult<int[]?>(new[] { 1, 1 }) : new(pending.Task);
        public ValueTask SetValueAndCaretAsync(ElementReference element, string value, int start, int end)
        { CaretWrites++; return ValueTask.CompletedTask; }
        public ValueTask SelectRangeAsync(ElementReference element, int start, int end)
        { CaretWrites++; return ValueTask.CompletedTask; }
        public ValueTask FocusAndSelectAsync(ElementReference element) => ValueTask.CompletedTask;
        public ValueTask SelectAsync(ElementReference element) => ValueTask.CompletedTask;
        public ValueTask BlurAsync(ElementReference element) => ValueTask.CompletedTask;
        public ValueTask<ElementBounds> GetBoundsAsync(ElementReference element) => ValueTask.FromResult(default(ElementBounds));
    }
}

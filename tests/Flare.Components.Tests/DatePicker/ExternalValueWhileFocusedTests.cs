using System.Globalization;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-163: a value the parent sets while the field has focus wins over an input still in flight (its caret
/// read is awaited), and the focused field shows that value in its editing form. A parent echoing the value the
/// field just committed changes nothing.
/// </summary>
public class ExternalValueWhileFocusedTests : FlareTestContext
{
    private static readonly CultureInfo Ru = CultureInfo.GetCultureInfo("ru-RU");

    [Fact]
    public async Task Date_ExternalValue_CancelsThePendingInput()
    {
        var gate = new SelectionGate();
        Services.AddSingleton<IElementJsService>(gate);
        DateOnly? committed = null;
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateOnly(2026, 10, 15)).Add(x => x.ValueChanged, (DateOnly? v) => committed = v));
        cut.Find("input").Focus();

        var pending = cut.Find("input").InputAsync(new ChangeEventArgs { Value = "16102026" });
        cut.Render(p => p.Add(x => x.Value, new DateOnly(2027, 2, 3)));
        gate.Release();
        await pending;

        Assert.Null(committed);
        Assert.Equal("03.02.2027", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public async Task DateTime_ExternalValue_CancelsThePendingInput()
    {
        var gate = new SelectionGate();
        Services.AddSingleton<IElementJsService>(gate);
        DateTimeOffset? committed = null;
        var cut = Render<FlareDateTimePicker>(p => p.Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateTimeOffset(2026, 10, 15, 12, 30, 0, TimeSpan.FromHours(5)))
            .Add(x => x.ValueChanged, (DateTimeOffset? v) => committed = v));
        cut.Find("input").Focus();

        var pending = cut.Find("input").InputAsync(new ChangeEventArgs { Value = "161020261230" });
        cut.Render(p => p.Add(x => x.Value, new DateTimeOffset(2027, 2, 3, 8, 15, 0, TimeSpan.FromHours(5))));
        gate.Release();
        await pending;

        Assert.Null(committed);
        Assert.Equal("03.02.2027 08:15", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Date_ParentEchoOfTheCommittedValue_KeepsTheTypedText()
    {
        DateOnly? committed = null;
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Culture, Ru).Add(x => x.Value, new DateOnly(2026, 10, 15))
            .Add(x => x.ValueChanged, (DateOnly? v) => committed = v));
        cut.Find("input").Focus();

        cut.Find("input").Input("16102026");
        cut.Render(p => p.Add(x => x.Value, committed));   // the parent binds the value back

        Assert.Equal(new DateOnly(2026, 10, 16), committed);
        Assert.Equal("16.10.2026", cut.Find("input").GetAttribute("value"));
    }

    private sealed class SelectionGate : IElementJsService
    {
        private readonly TaskCompletionSource<int[]?> _pending = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public void Release() => _pending.TrySetResult([12, 12]);
        public ValueTask<int[]?> GetSelectionAsync(ElementReference element) => new(_pending.Task);
        public ValueTask SetValueAndCaretAsync(ElementReference element, string value, int start, int end) => ValueTask.CompletedTask;
        public ValueTask SelectRangeAsync(ElementReference element, int start, int end) => ValueTask.CompletedTask;
        public ValueTask FocusAndSelectAsync(ElementReference element) => ValueTask.CompletedTask;
        public ValueTask SelectAsync(ElementReference element) => ValueTask.CompletedTask;
        public ValueTask BlurAsync(ElementReference element) => ValueTask.CompletedTask;
        public ValueTask<ElementBounds> GetBoundsAsync(ElementReference element) => ValueTask.FromResult(default(ElementBounds));
    }
}

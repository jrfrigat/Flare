using System.Globalization;
using Bunit;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-138: an input handler waits for the caret position from JS. When the field was changed, left,
/// edited again or re-synced from Value meanwhile, the late answer must not write the half-edited text
/// back or commit it.
/// </summary>
public class MaskedStaleSelectionTests : FlareTestContext
{
    private static readonly CultureInfo Ru = CultureInfo.GetCultureInfo("ru-RU");

    private GatedSelection UseGate()
    {
        var gate = new GatedSelection();
        Services.AddSingleton<IElementJsService>(gate);
        return gate;
    }

    private static async Task LeaveWhileSelectionIsPending<T>(IRenderedComponent<T> cut, GatedSelection gate, string raw)
        where T : IComponent
    {
        cut.Find("input").Focus();
        var pending = cut.Find("input").InputAsync(new ChangeEventArgs { Value = raw });
        cut.Find("input").Change(raw);
        cut.Find("input").Blur();
        gate.ReleaseAll();
        await pending;
    }

    [Fact]
    public async Task DatePicker_LateSelectionAfterBlur_KeepsTheRestoredText()
    {
        var gate = UseGate();
        var calls = 0;
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateOnly(2026, 10, 15)).Add(x => x.ValueChanged, (DateOnly? _) => calls++));

        await LeaveWhileSelectionIsPending(cut, gate, "1.10.2026");

        Assert.Equal("15.10.2026", cut.Find("input").GetAttribute("value"));
        Assert.Equal(0, calls);
    }

    [Fact]
    public async Task TimePicker_LateSelectionAfterBlur_KeepsTheRestoredText()
    {
        var gate = UseGate();
        var calls = 0;
        var cut = Render<FlareTimePicker>(p => p
            .Add(x => x.Value, new TimeOnly(12, 33)).Add(x => x.ValueChanged, (TimeOnly? _) => calls++));

        await LeaveWhileSelectionIsPending(cut, gate, "1:33");

        Assert.Equal("12:33", cut.Find("input").GetAttribute("value"));
        Assert.Equal(0, calls);
    }

    [Fact]
    public async Task DateTimePicker_LateSelectionAfterBlur_KeepsTheRestoredText()
    {
        var gate = UseGate();
        var calls = 0;
        var cut = Render<FlareDateTimePicker>(p => p.Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateTimeOffset(2026, 10, 15, 12, 30, 0, TimeSpan.FromHours(3)))
            .Add(x => x.ValueChanged, (DateTimeOffset? _) => calls++));

        await LeaveWhileSelectionIsPending(cut, gate, "1.10.2026 12:30");

        Assert.Equal("15.10.2026 12:30", cut.Find("input").GetAttribute("value"));
        Assert.Equal(0, calls);
    }

    [Fact]
    public async Task OlderInputAnsweredLast_DoesNotOverwriteTheNewerText()
    {
        var gate = UseGate();
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Culture, Ru).Add(x => x.Value, new DateOnly(2026, 10, 15)));
        cut.Find("input").Focus();

        var first = cut.Find("input").InputAsync(new ChangeEventArgs { Value = "1.10.2026" });
        var second = cut.Find("input").InputAsync(new ChangeEventArgs { Value = "16.10.2026" });
        gate.Release(1);
        await second;
        gate.Release(0);
        await first;

        Assert.Equal("16.10.2026", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public async Task ExternalValueWhilePending_WinsOverTheLateAnswer()
    {
        var gate = UseGate();
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.Value, new TimeOnly(12, 33)));
        cut.Find("input").Focus();

        var pending = cut.Find("input").InputAsync(new ChangeEventArgs { Value = "1:33" });
        cut.Render(p => p.Add(x => x.Value, new TimeOnly(8, 15)));
        gate.ReleaseAll();
        await pending;

        Assert.Equal("08:15", cut.Find("input").GetAttribute("value"));
    }

    private sealed class GatedSelection : IElementJsService
    {
        private readonly List<TaskCompletionSource<int[]?>> _requests = new();

        public void Release(int index) => _requests[index].TrySetResult(new[] { 1, 1 });
        public void ReleaseAll() { foreach (var r in _requests) r.TrySetResult(new[] { 1, 1 }); }

        public ValueTask<int[]?> GetSelectionAsync(ElementReference element)
        {
            var request = new TaskCompletionSource<int[]?>(TaskCreationOptions.RunContinuationsAsynchronously);
            _requests.Add(request);
            return new(request.Task);
        }
        public ValueTask SetValueAndCaretAsync(ElementReference element, string value, int start, int end) => ValueTask.CompletedTask;
        public ValueTask SelectRangeAsync(ElementReference element, int start, int end) => ValueTask.CompletedTask;
        public ValueTask FocusAndSelectAsync(ElementReference element) => ValueTask.CompletedTask;
        public ValueTask SelectAsync(ElementReference element) => ValueTask.CompletedTask;
        public ValueTask BlurAsync(ElementReference element) => ValueTask.CompletedTask;
        public ValueTask<ElementBounds> GetBoundsAsync(ElementReference element) => ValueTask.FromResult(default(ElementBounds));
    }
}

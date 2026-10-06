using System.Globalization;
using System.Linq.Expressions;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

/// <summary>Pending caret reads and queued text changes must not publish after input is locked.</summary>
public class PickerInputLockTests : FlareTestContext
{
    private static readonly CultureInfo Ru = CultureInfo.GetCultureInfo("ru-RU");

    [Theory]
    [InlineData(0, 0)] [InlineData(0, 1)] [InlineData(0, 2)]
    [InlineData(1, 0)] [InlineData(1, 1)] [InlineData(1, 2)]
    [InlineData(2, 0)] [InlineData(2, 1)]
    [InlineData(3, 0)] [InlineData(3, 1)] [InlineData(3, 2)]
    public Task LockDuringCaretRead_CancelsPendingInput(int family, int mode) => Pending(family, mode, false);

    [Theory]
    [InlineData(0)] [InlineData(1)] [InlineData(2)] [InlineData(3)]
    public Task LockThenUnlock_StillCancelsTheOldInput(int family) => Pending(family, 0, true);

    private Task Pending(int family, int mode, bool unlock) => family switch
    {
        0 => Pending<FlareDatePicker, DateOnly?>(new(2026, 10, 15), "16102026", mode, unlock),
        1 => Pending<FlareDateTimePicker, DateTimeOffset?>(new(2026, 10, 15, 12, 30, 0, TimeSpan.FromHours(5)), "161020261230", mode, unlock),
        2 => Pending<FlareTimePicker, TimeOnly?>(new(12, 30), "1437", mode, unlock),
        _ => Pending<FlareMonthPicker, DateOnly?>(new(2026, 10, 1), "112026", mode, unlock),
    };

    private async Task Pending<T, TValue>(TValue value, string raw, int mode, bool unlock) where T : FlareFieldBase
    {
        var gate = new SelectionGate();
        Services.AddSingleton<IElementJsService>(gate);
        var published = false;
        var cut = Render<T>(p => p.Add(Parameter<T, CultureInfo?>("Culture"), Ru).Add(Parameter<T, TValue>("Value"), value)
            .Add(Parameter<T, EventCallback<TValue>>("ValueChanged"), EventCallback.Factory.Create<TValue>(this, _ => published = true)));
        cut.Find("input").Focus();
        var before = cut.Find("input").GetAttribute("value");
        var pending = cut.Find("input").InputAsync(new ChangeEventArgs { Value = raw });
        Lock(cut, mode, true);
        if (unlock) Lock(cut, mode, false);
        gate.Release();
        await pending;
        Assert.False(published);
        Assert.Equal(before, cut.Find("input").GetAttribute("value"));
    }

    [Theory]
    [InlineData(0, 0)] [InlineData(0, 1)] [InlineData(0, 2)]
    [InlineData(1, 0)] [InlineData(1, 1)] [InlineData(1, 2)]
    [InlineData(2, 0)] [InlineData(2, 1)]
    [InlineData(3, 0)] [InlineData(3, 1)] [InlineData(3, 2)]
    [InlineData(4, 0)] [InlineData(4, 1)] [InlineData(4, 2)]
    [InlineData(5, 0)] [InlineData(5, 1)] [InlineData(5, 2)]
    public void LockedChangeAndClear_AreIgnoredButExternalValueStillApplies(int family, int mode)
    {
        switch (family)
        {
            case 0: Queued<FlareDatePicker, DateOnly?>(new(2026, 10, 15), new(2026, 10, 16), "16.10.2026", mode); break;
            case 1: Queued<FlareDateTimePicker, DateTimeOffset?>(new(2026, 10, 15, 12, 30, 0, TimeSpan.Zero), new(2026, 10, 16, 14, 37, 0, TimeSpan.Zero), "16.10.2026 14:37", mode); break;
            case 2: Queued<FlareTimePicker, TimeOnly?>(new(12, 30), new(14, 37), "14:37", mode); break;
            case 3: Queued<FlareMonthPicker, DateOnly?>(new(2026, 10, 1), new(2026, 11, 1), "11.2026", mode); break;
            case 4: Queued<FlareWeekPicker, DateOnly?>(new(2026, 10, 12), new(2026, 10, 19), "2026-W42", mode); break;
            default: Queued<FlareMultiDatePicker, IReadOnlyList<DateOnly>>([new(2026, 10, 15)], [new(2026, 10, 16)], "16.10.2026", mode, multi: true); break;
        }
    }

    private void Queued<T, TValue>(TValue value, TValue next, string raw, int mode, bool multi = false) where T : FlareFieldBase
    {
        var published = false;
        var name = multi ? "Values" : "Value";
        var cut = Render<T>(p => p.Add(Parameter<T, CultureInfo?>("Culture"), Ru).Add(Parameter<T, TValue>(name), value)
            .Add(Parameter<T, EventCallback<TValue>>(name + "Changed"), EventCallback.Factory.Create<TValue>(this, _ => published = true)));
        Lock(cut, mode, true);
        var before = cut.Find("input").GetAttribute("value");
        cut.Find("input").Change(raw);
        cut.Find("input").Change("");
        Assert.False(published);
        Assert.Equal(before, cut.Find("input").GetAttribute("value"));
        cut.Render(p => p.Add(Parameter<T, TValue>(name), next));
        Assert.NotEqual(before, cut.Find("input").GetAttribute("value"));
    }

    private static void Lock<T>(IRenderedComponent<T> cut, int mode, bool locked) where T : FlareFieldBase
    {
        if (mode == 0) cut.Render(p => p.Add(x => x.Disabled, locked));
        else if (mode == 1) cut.Render(p => p.Add(x => x.ReadOnly, locked));
        else cut.Render(p => p.Add(Parameter<T, bool>("AllowInput"), !locked));
    }

    private static Expression<Func<T, TValue>> Parameter<T, TValue>(string name)
    {
        var component = Expression.Parameter(typeof(T), "component");
        return Expression.Lambda<Func<T, TValue>>(Expression.Property(component, name), component);
    }

    [Fact]
    public void AllowInputFalse_StillAllowsCalendarSelection()
    {
        DateOnly? committed = null;
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.AllowInput, false)
            .Add(x => x.Value, new DateOnly(2026, 10, 15)).Add(x => x.ValueChanged, (DateOnly? v) => committed = v));
        cut.FindAll("button[role=gridcell]").First(b => b.GetAttribute("aria-selected") == "true").Click();
        Assert.Equal(new DateOnly(2026, 10, 15), committed);
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

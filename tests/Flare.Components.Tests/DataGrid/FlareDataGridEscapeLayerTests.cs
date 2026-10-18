using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// The two popups a grid opens - the column picker and the column filter menu - register as overlay layers while
/// open, so Escape closes the popup and leaves a dialog around the grid alone. The ordering lives in the overlay
/// script; this pins what the grid hands it.
/// </summary>
public sealed class FlareDataGridEscapeLayerTests : FlareTestContext
{
    private readonly RecordingRuntime _runtime;

    public FlareDataGridEscapeLayerTests()
    {
        _runtime = new RecordingRuntime(JSInterop.JSRuntime);
        Services.AddScoped<IOverlayJsService>(_ => new OverlayJsService(_runtime));
    }

    private record Fruit(string Name);

    private static readonly Fruit[] _items = [new("Apple"), new("Banana"), new("Cherry")];

    private IRenderedComponent<FlareDataGrid<Fruit>> RenderGrid(bool filterMenu = false) =>
        Render<FlareDataGrid<Fruit>>(p =>
        {
            p.Add(x => x.Items, _items.AsEnumerable());
            p.Add(x => x.ShowColumnPicker, !filterMenu);
            if (filterMenu) p.Add(x => x.FilterMode, DataGridFilterMode.Menu);
            p.Add(x => x.Columns, (RenderFragment)(inner =>
            {
                inner.OpenComponent<FlareColumn<Fruit>>(10);
                inner.AddAttribute(11, "Title", "Name");
                inner.AddAttribute(12, "Field", (Func<Fruit, object?>)(f => f.Name));
                inner.AddAttribute(13, "Filterable", filterMenu);
                inner.CloseComponent();
            }));
        });

    [Fact]
    public void ColumnPicker_OpeningRegistersTheLayerAndClosingRemovesIt()
    {
        var cut = RenderGrid();

        cut.Find($".{Css.Classes.DataGrid.ColumnPickerWrap} button").Click();
        cut.WaitForAssertion(() => Assert.Contains("pushPopupLayer", Names()));

        cut.Find($".{Css.Classes.DataGrid.Backdrop}").Click();
        cut.WaitForAssertion(() => Assert.Contains("removePopupLayer", Names()));

        Assert.True(Names().IndexOf("removePopupLayer") > Names().IndexOf("pushPopupLayer"),
            "The picker removed its layer before registering it: " + string.Join(", ", Names()));
    }

    [Fact]
    public async Task ColumnPicker_EscapeClosesThePanelAndRemovesTheLayer()
    {
        var cut = RenderGrid();

        cut.Find($".{Css.Classes.DataGrid.ColumnPickerWrap} button").Click();
        cut.WaitForAssertion(() => Assert.Contains("pushPopupLayer", Names()));

        await InvokeEscapeAsync("CloseFromEscape");

        cut.WaitForAssertion(() => Assert.Empty(cut.FindAll($".{Css.Classes.DataGrid.ColumnPickerOpen}")));
        cut.WaitForAssertion(() => Assert.Contains("removePopupLayer", Names()));
    }

    [Fact]
    public void ColumnPicker_RemovedFromTheTreeReleasesItsLayer()
    {
        var cut = RenderGrid();

        cut.Find($".{Css.Classes.DataGrid.ColumnPickerWrap} button").Click();
        cut.WaitForAssertion(() => Assert.Contains("pushPopupLayer", Names()));

        cut.Render(p => p.Add(x => x.ShowColumnPicker, false));

        cut.WaitForAssertion(() => Assert.Contains("removePopupLayer", Names()));
    }

    [Fact]
    public void FilterMenu_OpeningRegistersTheLayerAndClosingRemovesIt()
    {
        var cut = RenderGrid(filterMenu: true);

        cut.Find($"button.{Css.Classes.DataGrid.FilterTrigger}").Click();
        cut.WaitForAssertion(() => Assert.Contains("pushPopupLayer", Names()));

        cut.Find($"button.{Css.Classes.DataGrid.FilterTrigger}").Click();
        cut.WaitForAssertion(() => Assert.Contains("removePopupLayer", Names()));

        Assert.True(Names().IndexOf("removePopupLayer") > Names().IndexOf("pushPopupLayer"),
            "The filter menu removed its layer before registering it: " + string.Join(", ", Names()));
    }

    [Fact]
    public async Task FilterMenu_EscapeClosesTheMenuAndRemovesTheLayer()
    {
        var cut = RenderGrid(filterMenu: true);

        cut.Find($"button.{Css.Classes.DataGrid.FilterTrigger}").Click();
        cut.WaitForAssertion(() => Assert.Contains("pushPopupLayer", Names()));

        await InvokeEscapeAsync("CloseFilterMenuFromEscape");

        cut.WaitForAssertion(() => Assert.Empty(cut.FindAll($".{Css.Classes.DataGrid.FilterMenu}")));
        cut.WaitForAssertion(() => Assert.Contains("removePopupLayer", Names()));
    }

    // The two components are generic, so the layer's DotNetObjectReference<T> is read through its public Value.
    private async Task InvokeEscapeAsync(string methodName)
    {
        var push = _runtime.Calls.Last(c => c.Name == "pushPopupLayer");
        Assert.Equal(methodName, push.Args![2]);
        var dotNetRef = push.Args[1]!;
        var instance = dotNetRef.GetType().GetProperty("Value")!.GetValue(dotNetRef)!;
        var method = instance.GetType().GetMethod(methodName)!;
        await (Task)method.Invoke(instance, null)!;
    }

    private List<string> Names() => _runtime.Calls.Select(c => c.Name).ToList();

    // Records the module method calls with their arguments; the ordinary bUnit module reference keeps neither.
    private sealed class RecordingRuntime(IJSRuntime runtime) : IJSRuntime
    {
        public List<(string Name, object?[]? Args)> Calls { get; } = [];

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, default, args);

        public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken,
            object?[]? args)
        {
            if (typeof(TValue) != typeof(IJSObjectReference))
                return await runtime.InvokeAsync<TValue>(identifier, cancellationToken, args);

            var reference = await runtime.InvokeAsync<IJSObjectReference>("import", cancellationToken, args);
            return (TValue)(object)new RecordingReference(reference, Calls);
        }

        private sealed class RecordingReference(IJSObjectReference inner, List<(string Name, object?[]? Args)> calls)
            : IJSObjectReference
        {
            public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            {
                calls.Add((identifier, args));
                return inner.InvokeAsync<TValue>(identifier, args);
            }

            public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken,
                object?[]? args)
            {
                calls.Add((identifier, args));
                return inner.InvokeAsync<TValue>(identifier, cancellationToken, args);
            }

            public ValueTask DisposeAsync() => inner.DisposeAsync();
        }
    }
}

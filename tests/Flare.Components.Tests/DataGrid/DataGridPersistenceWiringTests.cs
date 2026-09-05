using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class DataGridPersistenceWiringTests : FlareTestContext
{
    private record Row(string Name, int Score);

    private static readonly Row[] _rows =
    [
        new("Bob", 2),
        new("Alice", 1),
        new("Carol", 3),
    ];

    private static RenderFragment Columns() => b =>
    {
        b.OpenComponent<FlareColumn<Row>>(0);
        b.AddAttribute(1, "Title", "Name");
        b.AddAttribute(2, "Field", (Func<Row, object?>)(r => r.Name));
        b.AddAttribute(3, "Sortable", true);
        b.CloseComponent();
    };

    [Fact]
    public void FreshGrid_PersistsFirstUserChange_EvenWithEmptyStorage()
    {
        // Loose JS interop returns null for localStorage.getItem -> storage starts empty.
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.PersistStateKey, "wiring-key")
            .Add(x => x.Columns, Columns()));

        // Sorting is a user change and must be persisted even with nothing previously stored.
        cut.FindAll("thead th").First(th => th.TextContent.Contains("Name")).Click();

        cut.WaitForAssertion(() => Assert.Contains(
            JSInterop.Invocations["localStorage.setItem"],
            i => i.Arguments.Count > 0 && i.Arguments[0] as string == "wiring-key"));
    }

    [Fact]
    public void Restore_AppliesSavedPageSize_NotClobberedByDefault()
    {
        // LoadAsync reads this saved state (page size 8) back from storage on init.
        const string json = "{\"sorts\":[],\"filters\":{},\"columnOrder\":[],\"hiddenColumns\":[],\"page\":0,\"pageSize\":8}";
        JSInterop.Setup<string?>("localStorage.getItem", "restore-key").SetResult(json);

        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.PageSize, 5)
            .Add(x => x.PersistStateKey, "restore-key")
            .Add(x => x.Columns, Columns()));

        // Regression: OnParametersSet runs after the restore and used to reset _currentPageSize to
        // PageSize (5), discarding the persisted size. The restored 8 must win.
        Assert.Equal(8, cut.Instance.EffectivePageSize);
    }
}

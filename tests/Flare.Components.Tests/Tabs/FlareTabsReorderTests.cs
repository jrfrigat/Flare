using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareTabsReorderTests : FlareTestContext
{
    private static void AddTabs(RenderTreeBuilder b, params string[] keys)
    {
        for (var i = 0; i < keys.Length; i++)
        {
            b.OpenComponent<FlareTab>(i * 3);
            b.AddAttribute(i * 3 + 1, nameof(FlareTab.Key), keys[i]);
            b.AddAttribute(i * 3 + 2, nameof(FlareTab.Label), keys[i].ToUpperInvariant());
            b.CloseComponent();
        }
    }

    private IRenderedComponent<FlareTabs> RenderTabs(IReadOnlyList<string>? order, Action<IReadOnlyList<string>>? changed = null,
        params string[] keys) => Render<FlareTabs>(p =>
    {
        p.Add(x => x.Reorderable, true);
        if (order is not null) p.Add(x => x.TabOrder, order);
        if (changed is not null) p.Add(x => x.TabOrderChanged, changed);
        p.AddChildContent(b => AddTabs(b, keys));
    });

    [Fact]
    public async Task OnTabReorder_MovesTheActiveTabAndReportsTheNewOrder()
    {
        IReadOnlyList<string>? order = null;
        var cut = RenderTabs(null, value => order = value, "a", "b", "c");

        await cut.InvokeAsync(() => cut.Instance.OnTabReorder("ArrowRight", false));

        Assert.Equal(new[] { "b", "a", "c" }, order);
        Assert.Equal(["B", "A", "C"], cut.FindAll($".{Css.Classes.Tabs.TabButton}").Select(tab => tab.TextContent.Trim()).ToArray());
        Assert.Equal("true", cut.FindAll("[role=tab]")[1].GetAttribute("aria-selected"));
    }

    [Fact]
    public async Task DragDrop_MovesTheDraggedTabToTheTargetPosition()
    {
        IReadOnlyList<string>? order = null;
        var cut = RenderTabs(null, value => order = value, "a", "b", "c");

        await cut.FindAll("[role=tab]")[0].TriggerEventAsync("ondragstart", new DragEventArgs());
        await cut.FindAll("[role=tab]")[2].TriggerEventAsync("ondrop", new DragEventArgs());

        Assert.Equal(new[] { "b", "c", "a" }, order);
        Assert.Equal(["B", "C", "A"], cut.FindAll($".{Css.Classes.Tabs.TabButton}").Select(tab => tab.TextContent.Trim()).ToArray());
    }

    [Fact]
    public void ControlledTabOrder_AppliesToTheBar()
    {
        var cut = RenderTabs(["c", "a", "b"], null, "a", "b", "c");

        Assert.Equal(["C", "A", "B"], cut.FindAll($".{Css.Classes.Tabs.TabButton}").Select(tab => tab.TextContent.Trim()).ToArray());
    }

    [Fact]
    public void Reorderable_ExposesTheAccessibleAffordances()
    {
        var cut = RenderTabs(null, null, "a", "b");

        var tab = cut.Find("[role=tab]");
        Assert.Equal("true", tab.GetAttribute("draggable"));
        Assert.Equal("Alt+ArrowLeft Alt+ArrowRight", tab.GetAttribute("aria-keyshortcuts"));
        Assert.Equal("true", cut.Find("[role=tablist]").GetAttribute("data-flare-tab-reorder"));
    }
}

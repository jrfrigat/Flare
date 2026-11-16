using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// The document strip can represent an explicit "no active document" selection while keeping the
/// uncontrolled first-tab default. A controlled strip takes its selection only from
/// <see cref="FlareDocumentTabs.ActiveKey"/>, so a Start Page or a side document can own the content
/// without a main tab looking active.
/// </summary>
public class IdeDocumentTabsSelectionTests : FlareTestContext
{
    public IdeDocumentTabsSelectionTests() => Services.AddFlareIde();

    private static RenderFragment Tabs(params (string Key, string Title)[] tabs) => builder =>
    {
        var sequence = 0;
        foreach (var (key, title) in tabs)
        {
            builder.OpenComponent<FlareDocumentTab>(sequence++);
            builder.SetKey(key);
            builder.AddAttribute(sequence++, nameof(FlareDocumentTab.Key), key);
            builder.AddAttribute(sequence++, nameof(FlareDocumentTab.Title), title);
            builder.CloseComponent();
        }
    };

    private static string[] Selected(IRenderedComponent<FlareDocumentTabs> cut) =>
        cut.FindAll("[role=tab]").Select(tab => tab.GetAttribute("aria-selected")!).ToArray();

    private static string[] Indices(IRenderedComponent<FlareDocumentTabs> cut) =>
        cut.FindAll("[role=tab]").Select(tab => tab.GetAttribute("tabindex")!).ToArray();

    [Fact]
    public async Task Uncontrolled_StartsOnFirstTabAndSelectsTheClickedTab()
    {
        string? reported = null;
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.ActiveKeyChanged, key => reported = key)
            .Add(x => x.ChildContent, Tabs(("a", "A.cs"), ("b", "B.cs"))));

        Assert.Equal(["true", "false"], Selected(cut));
        Assert.Equal(["0", "-1"], Indices(cut));
        Assert.Single(cut.FindAll("[role=tabpanel]"));

        await cut.FindAll("[role=tab]")[1].ClickAsync(new MouseEventArgs());

        Assert.Equal("b", reported);
        Assert.Equal(["false", "true"], Selected(cut));
    }

    [Fact]
    public void Controlled_WithNullActiveKey_ShowsNoActiveTab()
    {
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.StripOnly, true)
            .Add(x => x.ControlledSelection, true)
            .Add(x => x.ChildContent, Tabs(("a", "A.cs"), ("b", "B.cs"))));

        Assert.Equal(["false", "false"], Selected(cut));
        Assert.Equal(["0", "-1"], Indices(cut));
        Assert.Null(cut.Instance.ActiveTabTitle);
    }

    [Fact]
    public async Task Controlled_ReportsAClickWithoutSelectingUntilTheHostSetsTheKey()
    {
        string? reported = null;
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.StripOnly, true)
            .Add(x => x.ControlledSelection, true)
            .Add(x => x.ActiveKeyChanged, key => reported = key)
            .Add(x => x.ChildContent, Tabs(("a", "A.cs"), ("b", "B.cs"))));

        await cut.FindAll("[role=tab]")[0].ClickAsync(new MouseEventArgs());

        Assert.Equal("a", reported);
        Assert.Equal(["false", "false"], Selected(cut));

        cut.Render(p => p.Add(x => x.ActiveKey, "a"));

        Assert.Equal(["true", "false"], Selected(cut));
        Assert.Equal("A.cs", cut.Instance.ActiveTabTitle);
    }

    [Fact]
    public void Controlled_WithAKeyNamesExactlyThatTab()
    {
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.StripOnly, true)
            .Add(x => x.ControlledSelection, true)
            .Add(x => x.ActiveKey, "b")
            .Add(x => x.ChildContent, Tabs(("a", "A.cs"), ("b", "B.cs"))));

        Assert.Equal(["false", "true"], Selected(cut));
        Assert.Equal(["-1", "0"], Indices(cut));
        Assert.Equal("B.cs", cut.Instance.ActiveTabTitle);
    }

    [Fact]
    public void Controlled_WithAnUnknownKeyLeavesEveryTabInactive()
    {
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.StripOnly, true)
            .Add(x => x.ControlledSelection, true)
            .Add(x => x.ActiveKey, "missing")
            .Add(x => x.ChildContent, Tabs(("a", "A.cs"), ("b", "B.cs"))));

        Assert.Equal(["false", "false"], Selected(cut));
        Assert.Null(cut.Instance.ActiveTabTitle);
    }

    [Fact]
    public void Controlled_RemovingTheActiveTabLeavesNoSelection()
    {
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.StripOnly, true)
            .Add(x => x.ControlledSelection, true)
            .Add(x => x.ActiveKey, "a")
            .Add(x => x.ChildContent, Tabs(("a", "A.cs"), ("b", "B.cs"))));

        Assert.Equal(["true", "false"], Selected(cut));

        cut.Render(p => p.Add(x => x.ChildContent, Tabs(("b", "B.cs"))));

        Assert.Equal(["false"], Selected(cut));
        Assert.Null(cut.Instance.ActiveTabTitle);
    }

    [Fact]
    public void Controlled_WithoutSelectionRendersNoVisiblePanel()
    {
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.ControlledSelection, true)
            .Add(x => x.ChildContent, Tabs(("a", "A.cs"), ("b", "B.cs"))));

        Assert.Empty(cut.FindAll("[role=tabpanel]"));
    }

    [Fact]
    public async Task Controlled_NavigationFromNoSelectionReportsTheFirstTab()
    {
        string? reported = null;
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.StripOnly, true)
            .Add(x => x.ControlledSelection, true)
            .Add(x => x.ActiveKeyChanged, key => reported = key)
            .Add(x => x.ChildContent, Tabs(("a", "A.cs"), ("b", "B.cs"))));

        await cut.InvokeAsync(() => cut.Instance.OnTabNavigation("ArrowRight", false));

        Assert.Equal("a", reported);
        Assert.Equal(["false", "false"], Selected(cut));
    }

    [Fact]
    public async Task Controlled_MovesToTheKeyTheHostAccepts()
    {
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.ControlledSelection, true)
            .Add(x => x.ActiveKey, "first")
            .Add(x => x.ChildContent, Tabs(("first", "First.cs"), ("second", "Second.cs"))));

        await cut.FindAll("[role=tab]")[1].ClickAsync(new MouseEventArgs());

        Assert.Equal(["true", "false"], Selected(cut));

        cut.Render(p => p.Add(x => x.ActiveKey, "second"));

        Assert.Equal(["false", "true"], Selected(cut));
    }
}

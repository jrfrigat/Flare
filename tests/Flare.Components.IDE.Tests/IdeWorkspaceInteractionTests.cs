using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public sealed class IdeWorkspaceInteractionTests : FlareTestContext
{
    public IdeWorkspaceInteractionTests() => Services.AddFlareIde();

    [Fact]
    public async Task DocumentTabs_ReportKeysEvenForIdenticalTitles()
    {
        var active = "";
        var closed = "";
        var cut = Render<FlareDocumentTabs>(p => p
            .Add(x => x.ActiveKey, "second")
            .Add(x => x.ActiveKeyChanged, key => active = key)
            .Add(x => x.OnCloseRequested, key => closed = key)
            .AddChildContent<FlareDocumentTab>(t => t.Add(x => x.Key, "first").Add(x => x.Title, "main.cs"))
            .AddChildContent<FlareDocumentTab>(t => t.Add(x => x.Key, "second").Add(x => x.Title, "main.cs")));

        Assert.Equal(["false", "true"], cut.FindAll("[role=tab]").Select(t => t.GetAttribute("aria-selected")));
        await cut.FindAll("[role=tab]")[0].ClickAsync(new());
        await cut.FindAll(".flare-doc-tabs__close")[1].ClickAsync(new());

        Assert.Equal("first", active);
        Assert.Equal("second", closed);
        Assert.Equal("true", cut.FindAll("[role=tab]")[1].GetAttribute("aria-selected"));
    }

    [Fact]
    public void Toolbar_ReconcilesRestoredOrderAndDisablesAllMoveHandles()
    {
        RenderFragment content = builder => builder.AddContent(0, "Command");
        var cut = Render<FlareToolbar>(p => p
            .Add(x => x.Groups, [new("a", "A", content), new("b", "B", content), new("c", "C", content)])
            .Add(x => x.GroupOrder, ["missing", "b", "b", "a"])
            .Add(x => x.Reorderable, true)
            .Add(x => x.Disabled, true));

        Assert.Equal(["B", "A", "C"], cut.FindAll("[role=group]").Select(g => g.GetAttribute("aria-label")));
        Assert.All(cut.FindAll(".flare-toolbar__handle"), handle =>
        {
            Assert.True(handle.HasAttribute("disabled"));
            Assert.False(handle.HasAttribute("draggable"));
        });
    }

    [Fact]
    public void Toolbar_TrailingContentRendersAfterEveryGroupAndGetsNoHandle()
    {
        RenderFragment command = builder => builder.AddContent(0, "Command");
        RenderFragment leading = builder => builder.AddMarkupContent(0, "<span class=\"leading-slot\">Fixed</span>");
        RenderFragment trailing = builder => builder.AddMarkupContent(0, "<span class=\"trailing-slot\">Pinned</span>");
        var cut = Render<FlareToolbar>(p => p
            .Add(x => x.ChildContent, leading)
            .Add(x => x.Groups, [new("a", "A", command), new("b", "B", command)])
            .Add(x => x.TrailingContent, trailing)
            .Add(x => x.Reorderable, true));

        var children = cut.Find($".{Css.Classes.Ide.Toolbar.Root}").Children.ToArray();
        var trailingSlot = children[^1];
        var groups = children.Where(child => child.GetAttribute("role") == "group").ToArray();

        Assert.Equal("leading-slot", children[0].ClassName);
        Assert.Equal("trailing-slot", trailingSlot.ClassName);
        Assert.Equal("Pinned", trailingSlot.TextContent);
        Assert.Equal(2, groups.Length);
        Assert.All(groups, group => Assert.True(Array.IndexOf(children, group) < Array.IndexOf(children, trailingSlot)));
        Assert.Equal(groups.Length, cut.FindAll("[role=group]").Count);
        Assert.Null(trailingSlot.GetAttribute("role"));
        Assert.Empty(trailingSlot.QuerySelectorAll($".{Css.Classes.Ide.Toolbar.Handle}"));
        Assert.Equal(groups.Length, cut.FindAll($".{Css.Classes.Ide.Toolbar.Handle}").Count);
    }

    [Fact]
    public void Docking_KeepsTheToolPanelAutoHideModeUntilADocumentOccupiesItsRegion()
    {
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.Documents, [])
            .Add(x => x.LeftToolPanel, builder =>
            {
                builder.OpenComponent<FlareToolPanel>(0);
                builder.AddAttribute(1, nameof(FlareToolPanel.Mode), IdePanelMode.AutoHide);
                builder.CloseComponent();
            }));
        Assert.Single(cut.FindAll(".flare-ide-layout__panel--floating"));

        cut.Render(p => p.Add(x => x.Documents, [new("a", "Main.cs", IdeDocumentRegion.Left)]));

        Assert.Empty(cut.FindAll(".flare-ide-layout__panel--floating"));
        Assert.Single(cut.FindAll(".flare-ide-layout__document:not([hidden])"));
    }

    [Fact]
    public async Task DiagnosticList_FiltersTheVirtualizedGridBySeverity()
    {
        IdeDiagnostic first = new("1", "main", "Main.cs", 9, 3, IdeDiagnosticSeverity.Error, "Undefined name");
        IdeDiagnostic second = new("2", "guide", "Guide.md", 4, 1, IdeDiagnosticSeverity.Warning, "Missing heading");
        var cut = Render<FlareDiagnosticList>(p => p.Add(x => x.Items, [first, second]));
        var grid = cut.FindComponent<FlareDataGrid<IdeDiagnostic>>();

        Assert.True(grid.Instance.Virtual);
        Assert.Equal([first, second], grid.Instance.Items);
        await cut.InvokeAsync(() => cut.FindComponent<FlareSelect<string>>().Instance.ValueChanged.InvokeAsync("Error"));
        Assert.Equal([first], grid.Instance.Items);
    }

    [Fact]
    public async Task ShortcutEditor_RejectsAliasesAndReservedKeysAndCapturesWithinItsOwnScope()
    {
        string? value = null;
        var cut = Render<FlareShortcutEditor>(p => p
            .Add(x => x.AssignedShortcuts, ["Shift+Cmd+K"])
            .Add(x => x.ValueChanged, shortcut => value = shortcut));
        var field = cut.Find("input");
        await field.FocusInAsync(new());
        await field.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "K", CtrlKey = true, ShiftKey = true });
        Assert.Contains("already assigned", cut.Find("[role=status]").TextContent);
        Assert.Null(value);
        await field.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "l", CtrlKey = true });
        Assert.Contains("reserved", cut.Find("[role=status]").TextContent);
        await field.TriggerEventAsync("onkeydown",
            new KeyboardEventArgs { Key = "J", MetaKey = true, ShiftKey = true, AltKey = true });
        Assert.Equal("ctrl+alt+shift+j", value);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ShortcutEditor_DescribesErrorsWithTheHintHidden(bool compact)
    {
        var cut = Render<FlareShortcutEditor>(p => p
            .Add(x => x.Compact, compact)
            .Add(x => x.ShowStatus, false));
        var field = cut.Find("input");
        Assert.Null(field.GetAttribute("aria-describedby"));

        await field.FocusInAsync(new());
        await field.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "l", CtrlKey = true });

        var error = cut.Find("[role=status]");
        Assert.Contains("reserved", error.TextContent);
        Assert.Equal(error.Id, cut.Find("input").GetAttribute("aria-describedby"));

        await cut.Find("input").TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "j", CtrlKey = true });
        Assert.Empty(cut.FindAll("[role=status]"));
        Assert.Null(cut.Find("input").GetAttribute("aria-describedby"));
    }

    [Theory]
    [InlineData(null, "Keyboard shortcut")]
    [InlineData("Run query", "Run query")]
    public void ShortcutEditor_CompactFieldIsNamedWithoutAVisibleLabel(string? ariaLabel, string expectedName)
    {
        var cut = Render<FlareShortcutEditor>(p => p
            .Add(x => x.Compact, true)
            .Add(x => x.AriaLabel, ariaLabel));

        Assert.Empty(cut.FindAll("label"));
        Assert.Equal(expectedName, cut.Find("input").GetAttribute("aria-label"));
    }

    [Fact]
    public void ShortcutEditor_KeepsAnExplicitVisibleLabelInCompactMode()
    {
        var cut = Render<FlareShortcutEditor>(p => p
            .Add(x => x.Compact, true)
            .Add(x => x.Label, "Run query"));
        var field = cut.Find("input");
        var label = cut.Find("label");

        Assert.Equal("Run query", label.TextContent.Trim());
        Assert.Equal(field.Id, label.GetAttribute("for"));
        Assert.Null(field.GetAttribute("aria-label"));
    }
}
